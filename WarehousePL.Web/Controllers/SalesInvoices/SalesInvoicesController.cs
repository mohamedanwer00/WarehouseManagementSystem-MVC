using Microsoft.AspNetCore.Mvc;
using WarehouseBLL.BusinessServices.View_Models.SalesInvoice;
using WarehouseBLL.FormViewModels.SalesInvoice;
namespace WarehousePL.Web.Controllers.SalesInvoices;

public class SalesInvoicesController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStringLocalizer<SalesInvoicesController> _localization;
    public SalesInvoicesController(IUnitOfWork unitOfWork, IStringLocalizer<SalesInvoicesController> localization)
    {
        _unitOfWork = unitOfWork;
        _localization = localization;
    }

    public IActionResult Index()
    {
        var invoices = _unitOfWork.SalesInvoices
            .GetTableNoTracking()
            .Include(x => x.Customer)
            .Include(x => x.Branch)
            .Include(x => x.Warehouse)
            .OrderByDescending(x => x.Id)
            .ProjectToType<SalesInvoiceViewModel>()
            .ToList();

        return View(invoices);
    }

    [HttpGet]
    public IActionResult Create()
    {
        SalesInvoiceFormViewModel model = new()
        {
            InvoiceNumber = $"SI-{DateTime.Now:yyyyMMddHHmmss}",
            InvoiceDate = DateTime.Today
        };

        PopulateLists(model);
        return View(model);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SalesInvoiceFormViewModel model)
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

        // 1. التحقق من توفر الكميات في المخزن بالوحدة الأساسية
        foreach (var item in model.Items)
        {
            ProductUnit? productUnit = await _unitOfWork.ProductUnits.GetById(item.ProductUnitId);
            decimal factor = productUnit?.Factor ?? 1;

            // تحويل الكمية المطلوبة إلى الوحدة الأساسية
            decimal requiredBaseQuantity = (decimal)item.Quantity * factor;

            var stock = _unitOfWork.ProductWarehouses
                .AsQueryable()
                .FirstOrDefault(x => x.ProductId == item.ProductId && x.WarehouseId == model.WarehouseId);

            decimal availableQuantity = stock?.Quantity ?? 0;

            if (availableQuantity < requiredBaseQuantity)
            {
                var product = await _unitOfWork.Products.GetById(item.ProductId);

                // تحويل المتاح إلى الوحدة المطلوبة للعرض للمستخدم بشكل واضح
                decimal availableInSelectedUnit = factor > 0 ? Math.Round(availableQuantity / factor, 2) : availableQuantity;

                ModelState.AddModelError("", $"الكمية المتاحة من المنتج ({product?.Name ?? "المحدد"}) هي {availableInSelectedUnit} فقط بهذه الوحدة (أو {availableQuantity} بالوحدة الأساسية).");
                PopulateLists(model);
                return View(model);
            }
        }

        // 2. معالجة الدفع الكاش وزيادة الخزنة
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

            cashBox.CurrentBalance += model.Paid!.Value;
            _unitOfWork.CashBoxes.Update(cashBox);
        }

        SalesInvoice invoice = model.Adapt<SalesInvoice>();

        invoice.CreatedById = User.GetUserId();
        invoice.CreatedOn = DateTime.Now;
        invoice.LastAction = LastActionName.Insert;

        decimal paidAmount = invoice.Paid ?? 0;
        decimal totalAmount = invoice.TotalAmount;

        if (paidAmount >= totalAmount && totalAmount > 0)
            invoice.Status = InvoiceStatus.Paid;
        else if (paidAmount > 0 && paidAmount < totalAmount)
            invoice.Status = InvoiceStatus.PartiallyPaid;

        // 3. خصم الكميات من المخزن بالوحدة الأساسية
        foreach (SalesInvoiceItem item in invoice.SalesInvoiceItems)
        {
            item.CreatedById = User.GetUserId();
            item.CreatedOn = DateTime.Now;
            item.LastAction = LastActionName.Insert;

            item.TotalPrice = (item.SellingPrice * (decimal)item.Quantity) - (item.Discount ?? 0);
            ProductUnit? productUnit = await _unitOfWork.ProductUnits.GetById(item.ProductUnitId);
            decimal factor = productUnit?.Factor ?? 1;

            decimal baseQuantityToDeduct = (decimal)item.Quantity * factor;

            ProductWarehouse? stock = _unitOfWork.ProductWarehouses
                .AsQueryable()
                .FirstOrDefault(x => x.ProductId == item.ProductId && x.WarehouseId == invoice.WarehouseId);

            if (stock != null)
            {
                stock.Quantity -= baseQuantityToDeduct; // خصم الكمية بالوحدة الأساسية
                stock.CreatedById = User.GetUserId();
                stock.CreatedOn = DateTime.Now;
                _unitOfWork.ProductWarehouses.Update(stock);
            }
        }

        // 4. تحديث حساب العميل
        Customer? customer = await _unitOfWork.Customers.GetById(invoice.CustomerId);
        if (customer != null)
        {
            customer.CurrentBalance += (invoice.Remaining ?? 0);
            _unitOfWork.Customers.Update(customer);
        }

        await _unitOfWork.SalesInvoices.AddAsync(invoice);
        await _unitOfWork.SaveChangesAsync(); // الحفظ الأول لنحصل على invoice.Id

        // 1. CustomerTransaction
        await _unitOfWork.CustomerTransactions.AddAsync(new CustomerTransaction
        {
            CustomerId = invoice.CustomerId,
            Amount = invoice.Remaining ?? 0,
            BalanceType = BalanceType.Debitor,
            SalesInvoiceId = invoice.Id,
            Date = DateTime.Now
        });

        // 2. InventoryTransaction لكل صنف
        foreach (var item in invoice.SalesInvoiceItems)
        {
            ProductUnit? pu = await _unitOfWork.ProductUnits.GetById(item.ProductUnitId);
            decimal factor = (pu?.Factor ?? 0) != 0 ? pu!.Factor : 1;
            await _unitOfWork.InventoryTransactions.AddAsync(new InventoryTransaction
            {
                ProductId = item.ProductId,
                BranchId = invoice.BranchId,
                WarehouseId = invoice.WarehouseId,
                InventoryTransactionType = InventoryTransactionType.Sell,
                Quantity = (decimal)item.Quantity * factor,
                ReferenceNumber = invoice.InvoiceNumber,
                Date = DateTime.Now
            });
        }

        // 3. CashTransaction إذا كان الدفع كاش
        if (model.PaymentMethod == PaymentMethod.Cash && (invoice.Paid ?? 0) > 0)
        {
            await _unitOfWork.CashTransactions.AddAsync(new CashTransaction
            {
                CashBoxId = model.CashBoxId!.Value,
                Amount = invoice.Paid!.Value,
                TransactionType = CashTransactionType.Sales,
                ReferenceNumber = invoice.InvoiceNumber,
                Date = DateTime.Now
            });
        }

        await _unitOfWork.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Details(int id)
    {
        SalesInvoiceDetailsViewModel? invoice = _unitOfWork.SalesInvoices
            .GetTableNoTracking()
            .Include(x => x.Customer)
            .Include(x => x.Branch)
            .Include(x => x.Warehouse)
            .Include(x => x.SalesInvoiceItems)
                .ThenInclude(x => x.Product)
            .Include(x => x.SalesInvoiceItems)
                .ThenInclude(x => x.ProductUnit)
                    .ThenInclude(x => x.Unit)
            .Where(x => x.Id == id)
            .ProjectToType<SalesInvoiceDetailsViewModel>()
            .FirstOrDefault();

        if (invoice is null)
            return NotFound();

        return View(invoice);
    }

    private void PopulateLists(SalesInvoiceFormViewModel model)
    {
        model.Customers = _unitOfWork.Customers
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

        model.Items ??= new List<SalesInvoiceItemFormViewModel>();
    }

    private static void CalculateInvoiceTotals(SalesInvoiceFormViewModel model)
    {
        decimal itemsTotal = model.Items?.Sum(i =>
            (i.SellingPrice * i.Quantity) - (i.Discount ?? 0)) ?? 0;

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
    public async Task<IActionResult> GetSellingPrice(int productUnitId)
    {
        var unit = await _unitOfWork.ProductUnits.GetById(productUnitId);
        return Json(unit?.SellingPrice ?? 0);
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomerBalance(int customerId)
    {
        var customer = await _unitOfWork.Customers.GetById(customerId);
        return Json(customer?.CurrentBalance ?? 0);
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