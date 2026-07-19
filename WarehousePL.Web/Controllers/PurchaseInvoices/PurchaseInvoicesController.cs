using Microsoft.AspNetCore.Mvc;
using WarehouseBLL.BusinessServices.View_Models.PurchaseInvoice;
using WarehouseBLL.FormViewModels.PurchaseInvoice;
using WarehouseDAL.Entities.Enums;

namespace WarehousePL.Web.Controllers.PurchaseInvoices
{
    public class PurchaseInvoicesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public PurchaseInvoicesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ==================== INDEX ====================
        public IActionResult Index()
        {
            var invoices = _unitOfWork.PurchaseInvoices.GetTableNoTracking()
                    .Include(x => x.Supplier)
                    .Include(x => x.Warehouse)
                    .Include(x => x.Branch)
                    .Include(x => x.CashBox)
                    .OrderByDescending(x => x.InvoiceDate)
                    .ToList();

            var viewModel = invoices.Adapt<List<PurchaseInvoiceViewModel>>();
            return View(viewModel);
        }

        // ==================== CREATE GET ====================
        [HttpGet]
        public IActionResult Create()
        {
            var model = new PurchaseInvoiceFormViewModel
            {
                InvoiceNumber = GenerateInvoiceNumber(),
                InvoiceDate   = DateTime.Now
            };
            PopulateLists(model);
            return View(model);
        }

        // ==================== CREATE POST ====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PurchaseInvoiceFormViewModel model)
        {
            // تحقق من وجود عناصر
            if (model.Items == null || model.Items.Count == 0)
                ModelState.AddModelError("", "يجب إضافة عنصر واحد على الأقل للفاتورة.");

            // تحقق من الكاش بوكس لو الدفع كاش
            if (model.PaymentMethod == PaymentMethod.Cash && model.CashBoxId == null)
                ModelState.AddModelError(nameof(model.CashBoxId), "يجب اختيار الخزنة عند الدفع كاش.");

            // تحقق من رقم الفاتورة
            if (_unitOfWork.PurchaseInvoices.GetAll(x => x.InvoiceNumber == model.InvoiceNumber).Any())
                ModelState.AddModelError(nameof(model.InvoiceNumber), "رقم الفاتورة مستخدم بالفعل.");

            if (!ModelState.IsValid)
            {
                PopulateLists(model);
                return View(model);
            }

            // حساب الإجماليات
            model.Total     = model.Items!.Sum(i => i.Total);
            model.Net       = model.Total - model.Discount;
            model.Remaining = model.Net - model.Paid;

            // ضبط حالة الفاتورة
            var status = model.Paid == 0            ? InvoiceStatus.Draft
                       : model.Remaining <= 0        ? InvoiceStatus.Paid
                       : InvoiceStatus.PartiallyPaid;

            // بناء الـ entity
            var invoice = new PurchaseInvoice
            {
                InvoiceNumber = model.InvoiceNumber,
                InvoiceDate   = model.InvoiceDate,
                SupplierId    = model.SupplierId,
                BranchId      = model.BranchId,
                WarehouseId   = model.WarehouseId,
                CashBoxId     = model.PaymentMethod == PaymentMethod.Cash ? model.CashBoxId : null,
                PaymentMethod = model.PaymentMethod,
                Total         = model.Total,
                Discount      = model.Discount,
                Net           = model.Net,
                Paid          = model.Paid,
                Remaining     = model.Remaining,
                Status        = status,
                Notes         = model.Notes,
                LastAction    = LastActionName.Insert,
                CreatedOn     = DateTime.Now,
                PurchaseInvoiceItems = model.Items.Select(i => new PurchaseInvoiceItem
                {
                    ProductId  = i.ProductId,
                    UnitId     = i.UnitId,
                    Quantity   = i.Quantity,
                    UnitPrice  = i.UnitPrice,
                    Discount   = i.Discount,
                    Total      = i.Total,
                    LastAction = LastActionName.Insert,
                    CreatedOn  = DateTime.Now
                }).ToList()
            };

            _unitOfWork.PurchaseInvoices.Add(invoice);

            // تحديث المخزون — لكل عنصر في الفاتورة زود الكمية في ProductWarehouse
            foreach (var item in model.Items)
            {
                var stock = _unitOfWork.ProductWarehouse
                    .GetAll(x => x.ProductId == item.ProductId && x.WarehouseId == model.WarehouseId)
                    .FirstOrDefault();

                if (stock == null)
                {
                    // أول مرة يدخل الصنف ده للمخزن ده
                    _unitOfWork.ProductWarehouse.Add(new ProductWarehouse
                    {
                        ProductId   = item.ProductId,
                        WarehouseId = model.WarehouseId,
                        Quantity    = item.Quantity,
                        LastAction  = LastActionName.Insert,
                        CreatedOn   = DateTime.Now
                    });
                }
                else
                {
                    stock.Quantity  += item.Quantity;
                    stock.LastAction = LastActionName.Update;
                    stock.UpdatedOn  = DateTime.Now;
                    _unitOfWork.ProductWarehouse.Update(stock);
                }
            }

            // تحديث رصيد المورد (زيادة المديونية = رصيد سالب)
            var supplier = _unitOfWork.Suppliers.GetById(model.SupplierId);
            if (supplier != null)
            {
                supplier.CurrentBalance -= model.Remaining; // المتبقي = دين على الشركة للمورد
                _unitOfWork.Suppliers.Update(supplier);
            }

            _unitOfWork.SaveChanges();

            TempData["Message"] = "تم حفظ الفاتورة بنجاح.";
            return RedirectToAction(nameof(Details), new { id = invoice.Id });
        }

        // ==================== DETAILS ====================
        [HttpGet]
        public IActionResult Details(int id)
        {
            var invoice = _unitOfWork.PurchaseInvoices.GetTableNoTracking()
                .Include(x => x.Supplier)
                .Include(x => x.Branch)
                .Include(x => x.Warehouse)
                .Include(x => x.CashBox)
                .Include(x => x.PurchaseInvoiceItems)
                    .ThenInclude(i => i.Product)
                .Include(x => x.PurchaseInvoiceItems)
                    .ThenInclude(i => i.Unit)
                .FirstOrDefault(x => x.Id == id);

            if (invoice == null)
                return NotFound();

            return View(invoice);
        }

        // ==================== GET UNITS FOR PRODUCT (AJAX) ====================
        [HttpGet]
        public IActionResult GetProductUnits(int productId)
        {
            var units = _unitOfWork.ProductUnits.GetTableNoTracking()
                .Where(pu => pu.ProductId == productId)
                .Include(pu => pu.Unit)
                .Select(pu => new
                {
                    id           = pu.UnitId,
                    name         = pu.Unit != null ? pu.Unit.Name : "وحدة",
                    purchasePrice = pu.PurchasePrice,
                    isBaseUnit   = pu.IsBaseUnit
                })
                .ToList();

            return Json(units);
        }

        // ==================== GET ALL PRODUCTS (AJAX) ====================
        [HttpGet]
        public IActionResult GetAllProducts()
        {
            var products = _unitOfWork.Products.GetTableNoTracking()
                .Where(p => p.LastAction != LastActionName.Delete)
                .Select(p => new
                {
                    id   = p.Id,
                    name = p.Name,
                    code = p.Code
                })
                .ToList();

            return Json(products);
        }

        // ==================== HELPERS ====================
        private void PopulateLists(PurchaseInvoiceFormViewModel model)
        {
            model.Branches = _unitOfWork.Branches.GetTableNoTracking()
                .Where(x => x.LastAction != LastActionName.Delete)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

            model.Warehouses = _unitOfWork.Warehouses.GetTableNoTracking()
                .Where(x => x.LastAction != LastActionName.Delete)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

            model.Suppliers = _unitOfWork.Suppliers.GetTableNoTracking()
                .Where(x => x.LastAction != LastActionName.Delete)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

            model.CashBoxes = _unitOfWork.CashBoxes.GetTableNoTracking()
                .Where(x => x.LastAction != LastActionName.Delete)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
        }

        private string GenerateInvoiceNumber()
        {
            var last = _unitOfWork.PurchaseInvoices.GetAll()
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();

            int nextNum = (last?.Id ?? 0) + 1;
            return $"PINV-{DateTime.Now:yyyy}-{nextNum:D4}";
        }
    }
}
