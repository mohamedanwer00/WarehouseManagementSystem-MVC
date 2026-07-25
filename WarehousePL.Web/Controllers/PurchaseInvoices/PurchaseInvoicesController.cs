using WarehouseBLL.BusinessServices.View_Models.PurchaseInvoice;
using WarehouseBLL.FormViewModels.PurchaseInvoice;
using WarehouseDAL.Entities.Enums;

namespace WarehousePL.Web.Controllers.PurchaseInvoices;

public class PurchaseInvoicesController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStringLocalizer<PurchaseInvoicesController> _localization;

    public PurchaseInvoicesController(IUnitOfWork unitOfWork, IStringLocalizer<PurchaseInvoicesController> localization)
    {
        _unitOfWork = unitOfWork;
        _localization = localization;
    }

    public IActionResult Index()
    {
        var invoices = _unitOfWork.PurchaseInvoices
            .GetTableNoTracking()
            .Include(x => x.Supplier)
            .Include(x => x.Branch)
            .Include(x => x.Warehouse)
            .OrderByDescending(x => x.Id)
            .ProjectToType<PurchaseInvoiceViewModel>()
            .ToList();

        return View(invoices);
    }

    [HttpGet]
    public IActionResult Create()
    {
        PurchaseInvoiceFormViewModel model = new()
        {
            InvoiceNumber = $"PI-{DateTime.Now:yyyyMMddHHmmss}",
            InvoiceDate = DateTime.Today
        };

        PopulateLists(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PurchaseInvoiceFormViewModel model)
    {
        CalculateInvoiceTotals(model);

        if (!ModelState.IsValid || model.Items == null || !model.Items.Any())
        {
            if (model.Items == null || !model.Items.Any())
            {
                ModelState.AddModelError("", "يجب إضافة صنف واحد على الأقل.");
            }
            PopulateLists(model);
            return View(model);
        }

        if (model.PaymentMethod == PaymentMethod.Cash && (model.Paid ?? 0) > 0)
        {
            if (!model.CashBoxId.HasValue)
            {
                ModelState.AddModelError("", "يرجى تحديد الخزنة عند الدفع كاش.");
                PopulateLists(model);
                return View(model);
            }

            CashBox? cashBox = await _unitOfWork.CashBoxes.GetById(model.CashBoxId.Value);
            if (cashBox == null)
            {
                ModelState.AddModelError("", "الخزنة غير موجودة.");
                PopulateLists(model);
                return View(model);
            }

            if (cashBox.CurrentBalance < model.Paid!.Value)
            {
                ModelState.AddModelError("", "رصيد الخزنة لا يكفي لإتمام العملية.");
                PopulateLists(model);
                return View(model);
            }

            cashBox.CurrentBalance -= model.Paid.Value;
            _unitOfWork.CashBoxes.Update(cashBox);
        }

        PurchaseInvoice invoice = model.Adapt<PurchaseInvoice>();

        invoice.CreatedById = User.GetUserId();
        invoice.CreatedOn = DateTime.Now;
        invoice.LastAction = LastActionName.Insert;

        decimal paidAmount = invoice.Paid ?? 0;
        decimal totalAmount = invoice.TotalAmount;

        if (paidAmount >= totalAmount && totalAmount > 0)
            invoice.Status = InvoiceStatus.Paid;
        else if (paidAmount > 0 && paidAmount < totalAmount)
            invoice.Status = InvoiceStatus.PartiallyPaid;


        foreach (PurchaseInvoiceItem item in invoice.PurchaseInvoiceItems)
        {
            item.CreatedById = User.GetUserId();
            item.CreatedOn = DateTime.Now;
            item.LastAction = LastActionName.Insert;

            item.TotalPrice = (item.PurchasePrice * (decimal)item.Quantity) - (item.Discount ?? 0);

            // جلب الوحدة المحددة للحصول على الـ Factor
            ProductUnit? productUnit = await _unitOfWork.ProductUnits.GetById(item.ProductUnitId);
            decimal factor = productUnit?.Factor ?? 1;

            // حساب الكمية المحولة بالوحدة الأساسية (مثلاً 1 شكارة * 50 = 50 كيلو)
            decimal baseQuantity = (decimal)item.Quantity * factor;

            ProductWarehouse? stock = _unitOfWork.ProductWarehouses
                .AsQueryable()
                .FirstOrDefault(x => x.ProductId == item.ProductId && x.WarehouseId == invoice.WarehouseId);

            if (stock is null)
            {
                stock = new ProductWarehouse
                {
                    ProductId = item.ProductId,
                    WarehouseId = invoice.WarehouseId,
                    Quantity = baseQuantity, // إضافة بالوحدة الأساسية
                    CreatedById = User.GetUserId(),
                    CreatedOn = DateTime.Now,
                    LastAction = LastActionName.Insert
                };
                await _unitOfWork.ProductWarehouses.AddAsync(stock);
            }
            else
            {
                stock.Quantity += baseQuantity; // إضافة بالوحدة الأساسية
                stock.CreatedById = User.GetUserId();
                stock.CreatedOn = DateTime.Now;
                _unitOfWork.ProductWarehouses.Update(stock);
            }

            if (productUnit != null)
            {
                productUnit.PurchasePrice = item.PurchasePrice;
                _unitOfWork.ProductUnits.Update(productUnit);
            }
        }

        Supplier? supplier = await _unitOfWork.Suppliers.GetById(invoice.SupplierId);
        if (supplier != null)
        {
            supplier.CurrentBalance += (invoice.Remaining ?? 0);
            _unitOfWork.Suppliers.Update(supplier);
        }

        await _unitOfWork.PurchaseInvoices.AddAsync(invoice);
        _unitOfWork.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Details(int id)
    {
        PurchaseInvoiceDetailsViewModel? invoice = _unitOfWork.PurchaseInvoices
            .GetTableNoTracking()
            .Include(x => x.Supplier)
            .Include(x => x.Branch)
            .Include(x => x.Warehouse)
            .Include(x => x.PurchaseInvoiceItems)
                .ThenInclude(x => x.Product)
            .Include(x => x.PurchaseInvoiceItems)
                .ThenInclude(x => x.ProductUnit)
                    .ThenInclude(x => x.Unit)
            .Where(x => x.Id == id)
            .ProjectToType<PurchaseInvoiceDetailsViewModel>()
            .FirstOrDefault();

        if (invoice is null)
            return NotFound();

        return View(invoice);
    }

    private void PopulateLists(PurchaseInvoiceFormViewModel model)
    {
        model.Suppliers = _unitOfWork.Suppliers
            .GetAll(x => x.LastAction != LastActionName.Delete)
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name });

        model.Branches = _unitOfWork.Branches
            .GetAll(x => x.LastAction != LastActionName.Delete)
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name });

        model.Warehouses = model.BranchId > 0
            ? _unitOfWork.Warehouses
                .GetAll(x => x.BranchId == model.BranchId && x.LastAction != LastActionName.Delete)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name })
            : Enumerable.Empty<SelectListItem>();

        model.CashBoxes = model.BranchId > 0
            ? _unitOfWork.CashBoxes
                .GetAll(x => x.BranchId == model.BranchId && x.LastAction != LastActionName.Delete)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name })
            : Enumerable.Empty<SelectListItem>();

        model.PaymentMethods = Enum.GetValues(typeof(PaymentMethod))
            .Cast<PaymentMethod>()
            .Select(x => new SelectListItem { Value = ((int)x).ToString(), Text = x.ToString() });

        model.Items ??= new List<PurchaseInvoiceItemFormViewModel>();
    }

    private static void CalculateInvoiceTotals(PurchaseInvoiceFormViewModel model)
    {
        decimal itemsTotal = model.Items?.Sum(i =>
            (i.PurchasePrice * i.Quantity) - (i.Discount ?? 0)) ?? 0;

        decimal invoiceDiscount = model.Discount ?? 0;
        model.TotalAmount = itemsTotal - invoiceDiscount;
        if (model.TotalAmount < 0)
            model.TotalAmount = 0;

        model.Remaining = model.TotalAmount - (model.Paid ?? 0);
    }

    [HttpGet]
    public IActionResult GetWarehouses(int branchId)
    {
        var warehouses = _unitOfWork.Warehouses
            .GetAll(x => x.BranchId == branchId && x.LastAction != LastActionName.Delete)
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name })
            .ToList();

        return Json(warehouses);
    }

    [HttpGet]
    public IActionResult GetProducts()
    {
        var products = _unitOfWork.Products
            .GetAll(x => x.LastAction != LastActionName.Delete)
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name })
            .ToList();

        return Json(products);
    }

    [HttpGet]
    public async Task<IActionResult> GetUnits(int productId)
    {
        var units = await _unitOfWork.ProductUnits
            .GetTableNoTracking()
            .Where(x => x.ProductId == productId)
            .Select(x => new
            {
                value = x.Id,
                text = x.Unit!.Name,
                isDefault = x.IsBaseUnit
            })
            .ToListAsync();

        return Json(units);
    }

    [HttpGet]
    public async Task<IActionResult> GetPurchasePrice(int productUnitId)
    {
        var unit = await _unitOfWork.ProductUnits.GetById(productUnitId);
        return Json(unit?.PurchasePrice ?? 0);
    }

    [HttpGet]
    public async Task<IActionResult> GetSupplierBalance(int supplierId)
    {
        var supplier = await _unitOfWork.Suppliers.GetById(supplierId);
        return Json(supplier?.CurrentBalance ?? 0);
    }

    [HttpGet]
    public IActionResult GetCashBoxes(int branchId)
    {
        var cashBoxes = _unitOfWork.CashBoxes
            .GetAll(x => x.BranchId == branchId && x.LastAction != LastActionName.Delete)
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name })
            .ToList();

        return Json(cashBoxes);
    }
}