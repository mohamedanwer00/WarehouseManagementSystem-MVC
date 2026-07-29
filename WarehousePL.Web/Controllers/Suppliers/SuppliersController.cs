using WarehouseBLL.FormViewModels.Supplier;
namespace WarehousePL.Web.Controllers.Suppliers
{
    public class SuppliersController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public SuppliersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var suppliers = await _unitOfWork.Suppliers.GetTableNoTracking().ToListAsync();
            var viewModel = suppliers.Adapt<List<SupplierViewModel>>();
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new SupplierFormViewModel();
            return PartialView("_Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SupplierFormViewModel model)
        {
            if (await _unitOfWork.Suppliers.GetTableNoTracking()
                .AnyAsync(x => x.Name == model.Name && x.LastAction != LastActionName.Delete))
                ModelState.AddModelError(nameof(model.Name), "اسم المورد موجود بالفعل");

            if (await _unitOfWork.Suppliers.GetTableNoTracking()
                .AnyAsync(x => x.PhoneNumber == model.PhoneNumber && x.LastAction != LastActionName.Delete))
                ModelState.AddModelError(nameof(model.PhoneNumber), "رقم الهاتف مستخدم بالفعل");

            if (!ModelState.IsValid)
                return PartialView("_Form", model);

            var supplier = model.Adapt<Supplier>();
            supplier.LastAction = LastActionName.Insert;
            supplier.CreatedById = User.GetUserId();
            supplier.CreatedOn = DateTime.Now;
            supplier.CurrentBalance = model.OpeningBalance;

            await _unitOfWork.Suppliers.AddAsync(supplier);
            await _unitOfWork.SaveChangesAsync();

            if (model.OpeningBalance > 0)
            {
                await _unitOfWork.SupplierTransactions.AddAsync(new SupplierTransaction
                {
                    SupplierId = supplier.Id,
                    Amount = Math.Abs(model.OpeningBalance),
                    SupplierTransactionType = SupplierTransactionType.OPenBalance,
                    Notes = "رصيد افتتاحي",
                    Date = DateTime.Now
                });
                await _unitOfWork.SaveChangesAsync();
            }

            var viewModel = supplier.Adapt<SupplierViewModel>();
            viewModel.LastAction = supplier.LastAction;
            return PartialView("_Row", viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Supplier? supplier = await _unitOfWork.Suppliers.GetById(id);
            if (supplier is null)
                return NotFound();

            SupplierFormViewModel model = supplier.Adapt<SupplierFormViewModel>();
            return PartialView("_Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SupplierFormViewModel model)
        {
            if (await _unitOfWork.Suppliers.GetTableNoTracking()
                .AnyAsync(x => x.Name == model.Name && x.Id != model.Id && x.LastAction != LastActionName.Delete))
                ModelState.AddModelError(nameof(model.Name), "اسم المورد موجود بالفعل");

            if (await _unitOfWork.Suppliers.GetTableNoTracking()
                .AnyAsync(x => x.PhoneNumber == model.PhoneNumber && x.LastAction != LastActionName.Delete))
                ModelState.AddModelError(nameof(model.PhoneNumber), "رقم الهاتف مستخدم بالفعل");

            if (!ModelState.IsValid)
                return PartialView("_Form", model);

            var supplier = await _unitOfWork.Suppliers.GetById(model.Id!.Value);
            if (supplier == null)
                return NotFound();

            var oldOpeningBalance = supplier.OpeningBalance;
            var oldOpeningBalanceType = supplier.OpeningBalanceType;
            var oldCurrentBalance = supplier.CurrentBalance;

            model.Adapt(supplier);

            supplier.OpeningBalance = oldOpeningBalance;
            supplier.OpeningBalanceType = oldOpeningBalanceType;
            supplier.CurrentBalance = oldCurrentBalance;
            supplier.LastAction = LastActionName.Update;
            supplier.UpdatedById = User.GetUserId();
            supplier.UpdatedOn = DateTime.Now;

            _unitOfWork.Suppliers.Update(supplier);
            await _unitOfWork.SaveChangesAsync();

            var viewModel = supplier.Adapt<SupplierViewModel>();
            viewModel.LastAction = supplier.LastAction;
            return PartialView("_Row", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var supplier = await _unitOfWork.Suppliers.GetById(id);
            if (supplier is null)
                return NotFound();

            if (supplier.CurrentBalance != 0)
            {
                Response.StatusCode = 400;
                return Content("لا يمكن حذف المورد إلا إذا كان الرصيد الحالي يساوي صفر.");
            }

            supplier.LastAction = LastActionName.Delete;
            supplier.UpdatedById = User.GetUserId();
            supplier.UpdatedOn = DateTime.Now;

            _unitOfWork.Suppliers.Update(supplier);
            await _unitOfWork.SaveChangesAsync();

            var viewModel = supplier.Adapt<SupplierViewModel>();
            viewModel.LastAction = supplier.LastAction;
            return PartialView("_Row", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id)
        {
            var supplier = await _unitOfWork.Suppliers.GetById(id);
            if (supplier is null)
                return NotFound();

            supplier.LastAction = LastActionName.Update;
            supplier.UpdatedById = User.GetUserId();
            supplier.UpdatedOn = DateTime.Now;

            _unitOfWork.Suppliers.Update(supplier);
            await _unitOfWork.SaveChangesAsync();

            var rowViewModel = supplier.Adapt<SupplierViewModel>();
            rowViewModel.LastAction = supplier.LastAction;
            return PartialView("_Row", rowViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Statement(int? supplierId, DateTime? dateFrom, DateTime? dateTo)
        {
            dateFrom ??= DateTime.Today.AddMonths(-1);
            dateTo ??= DateTime.Today;

            var model = new SupplierStatementViewModel
            {
                SupplierId = supplierId ?? 0,
                DateFrom = dateFrom,
                DateTo = dateTo,
                Suppliers = await GetSuppliersListAsync()
            };

            if (supplierId.HasValue && supplierId > 0)
            {
                var supplier = await _unitOfWork.Suppliers.GetById(supplierId.Value);
                if (supplier != null)
                {
                    model.SupplierName = supplier.Name;
                    model.OpeningBalance = supplier.OpeningBalance;
                    model.OpeningBalanceType = supplier.OpeningBalanceType;

                    DateTime searchDateFrom = dateFrom.Value.Date;
                    DateTime searchDateTo = dateTo.Value.Date.AddDays(1).AddTicks(-1);

                    // 1. حساب المعاملات السابقة (قبل تاريخ البداية) لحساب الرصيد الافتتاحي المحاسبي الصحيح للفترة
                    var priorTransactions = await _unitOfWork.SupplierTransactions
                        .GetTableNoTracking()
                        .Where(t => t.SupplierId == supplierId.Value && t.Date < searchDateFrom)
                        .ToListAsync();

                    decimal priorBalance = priorTransactions.Sum(t =>
                        (t.SupplierTransactionType == SupplierTransactionType.PurchaseInvoice || t.SupplierTransactionType == SupplierTransactionType.OPenBalance)
                        ? t.Amount : -t.Amount);

                    // 2. جلب معاملات الفترة المحددة
                    var transactions = await _unitOfWork.SupplierTransactions
                        .GetTableNoTracking()
                        .Where(t => t.SupplierId == supplierId.Value && t.Date >= searchDateFrom && t.Date <= searchDateTo)
                        .Include(t => t.PurchaseInvoice)
                        .OrderBy(t => t.Date)
                        .ThenBy(t => t.Id)
                        .ToListAsync();

                    model.HasTransactions = transactions.Any();

                    // حساب الرصيد الافتتاحي الفعلي للفترة
                    decimal initialPeriodBalance = (model.OpeningBalanceType == BalanceType.Creditor ? model.OpeningBalance : -model.OpeningBalance) + priorBalance;

                    model.Lines = BuildStatementLines(transactions, initialPeriodBalance);

                    model.TotalDebtor = model.Lines.Sum(l => l.DebtorAmount ?? 0);
                    model.TotalCreditor = model.Lines.Sum(l => l.CreditAmount ?? 0);
                    model.ClosingBalance = model.Lines.LastOrDefault()?.RunningBalance ?? initialPeriodBalance;
                }
            }

            return View(model);
        }

        private async Task<IEnumerable<SelectListItem>> GetSuppliersListAsync()
        {
            return await _unitOfWork.Suppliers
                .GetTableNoTracking()
                .Where(x => x.LastAction != LastActionName.Delete)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name })
                .ToListAsync();
        }

        private static List<StatementLineViewModel> BuildStatementLines(
            List<SupplierTransaction> transactions,
            decimal initialBalance)
        {
            decimal running = initialBalance;
            var lines = new List<StatementLineViewModel>();

            foreach (var t in transactions)
            {
                string notes = t.Notes;
                if (t.SupplierTransactionType == SupplierTransactionType.OPenBalance && string.IsNullOrWhiteSpace(notes))
                    notes = "رصيد افتتاحي";
                else if (t.SupplierTransactionType == SupplierTransactionType.Payment && string.IsNullOrWhiteSpace(notes))
                    notes = "تم دفع";

                bool isPurchaseInvoice = t.SupplierTransactionType == SupplierTransactionType.PurchaseInvoice;

                if (t.SupplierTransactionType == SupplierTransactionType.PurchaseInvoice ||
                    t.SupplierTransactionType == SupplierTransactionType.OPenBalance)
                {
                    running += t.Amount;
                    lines.Add(new StatementLineViewModel
                    {
                        Date = t.Date,
                        Notes = notes,
                        CreditAmount = t.Amount,
                        RunningBalance = running,
                        InvoiceId = t.PurchaseInvoiceId,
                        InvoiceNumber = t.PurchaseInvoice?.InvoiceNumber,
                        IsPurchaseInvoice = isPurchaseInvoice
                    });
                }
                else if (t.SupplierTransactionType == SupplierTransactionType.Payment)
                {
                    running -= t.Amount;
                    lines.Add(new StatementLineViewModel
                    {
                        Date = t.Date,
                        Notes = notes,
                        DebtorAmount = t.Amount,
                        RunningBalance = running,
                        InvoiceId = t.PurchaseInvoiceId,
                        InvoiceNumber = t.PurchaseInvoice?.InvoiceNumber,
                        IsPurchaseInvoice = isPurchaseInvoice
                    });
                }
            }
            return lines;
        }

    }
}