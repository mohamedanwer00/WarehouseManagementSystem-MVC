using WarehouseBLL.BusinessServices.View_Models.CashBox;
using WarehouseBLL.FormViewModels.CashBox;
namespace WarehousePL.Web.Controllers.CashBoxes;
public class CashBoxesController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStringLocalizer<CashBoxesController> _localization;

    public CashBoxesController(IUnitOfWork unitOfWork, IStringLocalizer<CashBoxesController> localization)
    {
        _unitOfWork = unitOfWork;
        _localization = localization;
    }

    public async Task<IActionResult> Index()
    {
        var cashBoxes = await _unitOfWork.CashBoxes.AsQueryable().Include(c => c.Branch).ToListAsync();

        var viewModel = cashBoxes.Adapt<IEnumerable<CashBoxViewModel>>();
        return View(viewModel);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var branches = _unitOfWork.Branches.GetAll();
        var viewModel = new CashBoxFormViewModel
        {
            Branches = branches.Select(b => new SelectListItem
            {
                Text = b.Name,
                Value = b.Id.ToString()
            })
        };
        return PartialView("_Form", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CashBoxFormViewModel model)
    {
        var isNameExists = _unitOfWork.CashBoxes
            .GetAll()
            .Any(c => c.Name.Trim().ToLower() == model.Name.Trim().ToLower() && c.LastAction != LastActionName.Delete);

        if (isNameExists)
        {
            ModelState.AddModelError(nameof(model.Name), _localization["NameAlreadyExists"]);
        }

        if (!ModelState.IsValid)
        {
            var branches = _unitOfWork.Branches.GetAll();
            model.Branches = branches.Select(b => new SelectListItem
            {
                Text = b.Name,
                Value = b.Id.ToString()
            });
            return PartialView("_Form", model);
        }

            var cashBox = model.Adapt<CashBox>();
            cashBox.CurrentBalance = Math.Abs(model.OpeningBalance); // دائماً موجب
            cashBox.LastAction = LastActionName.Insert;
            cashBox.CreatedById = User.GetUserId();
            cashBox.CreatedOn = DateTime.Now;
            await _unitOfWork.CashBoxes.AddAsync(cashBox);
            await _unitOfWork.SaveChangesAsync();

            if (model.OpeningBalance > 0)
            {
                await _unitOfWork.CashTransactions.AddAsync(new CashTransaction
                {
                    CashBoxId = cashBox.Id,
                    Amount = Math.Abs(model.OpeningBalance), // دائماً موجب
                    TransactionType = CashTransactionType.OpeningBalance,
                    Notes = "رصيد افتتاحي",
                    Date = DateTime.Now
                });
                await _unitOfWork.SaveChangesAsync();
            }


            var viewModel = cashBox.Adapt<CashBoxViewModel>();
            viewModel.LastAction = cashBox.LastAction;

            var selectedBranch = await _unitOfWork.Branches.GetById(model.SelectedBranch);
            if (selectedBranch != null)
            {
                viewModel.BranchName = selectedBranch.Name;
            }

            return RedirectToAction(nameof(Index));
        

    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        CashBox? cashBox = await _unitOfWork.CashBoxes.GetById(id);

        if (cashBox == null)
            return NotFound();

        CashBoxFormViewModel viewModel = cashBox.Adapt<CashBoxFormViewModel>();

        viewModel.Branches = _unitOfWork.Branches.GetAll()
            .Select(b => new SelectListItem
            {
                Text = b.Name,
                Value = b.Id.ToString()
            });

        return PartialView("_Form", viewModel);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CashBoxFormViewModel model)
    {
        var cashBox = await _unitOfWork.CashBoxes.GetById(model.Id.Value);
        if (cashBox == null)
            return NotFound();

        var isNameExists = _unitOfWork.CashBoxes
            .GetAll()
            .Any(c => c.Id != model.Id && c.Name.Trim().ToLower() == model.Name.Trim().ToLower() && c.LastAction != LastActionName.Delete);

        if (isNameExists)
        {
            ModelState.AddModelError(nameof(model.Name), _localization["NameAlreadyExists"]);
        }
        if (!ModelState.IsValid)
        {
            var branches = _unitOfWork.Branches.GetAll();
            model.Branches = branches.Select(b => new SelectListItem
            {
                Text = b.Name,
                Value = b.Id.ToString()
            });
            return PartialView("_Form", model);
        }


            cashBox.Name = model.Name;
            cashBox.BranchId = model.SelectedBranch;
            // الا نغير OpeningBalance هنا لحماية الرصيد الحالي
            cashBox.LastAction = LastActionName.Update;
            cashBox.UpdatedById = User.GetUserId();
            cashBox.UpdatedOn = DateTime.Now;
            _unitOfWork.CashBoxes.Update(cashBox);
            await _unitOfWork.SaveChangesAsync();

            var viewModel = cashBox.Adapt<CashBoxViewModel>();
            viewModel.LastAction = cashBox.LastAction;

            var selectedBranch = await _unitOfWork.Branches.GetById(model.SelectedBranch);
            if (selectedBranch != null)
            {
                viewModel.BranchName = selectedBranch.Name;
            }
            return RedirectToAction(nameof(Index));
        
    }

    [HttpGet]
    public async Task<IActionResult> Deposit(int id)
    {
        var cashBox = await _unitOfWork.CashBoxes.GetById(id);
        if (cashBox == null || cashBox.LastAction == LastActionName.Delete)
            return NotFound();

        var model = new CashBoxTransactionFormViewModel
        {
            Id = cashBox.Id,
            Name = cashBox.Name
        };
        return PartialView("_DepositForm", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Deposit(CashBoxTransactionFormViewModel model)
    {
        var cashBox = await _unitOfWork.CashBoxes.GetById(model.Id);
        if (cashBox == null || cashBox.LastAction == LastActionName.Delete)
            return NotFound();
        model.Name = cashBox.Name;

        if (model.Amount <= 0)
            ModelState.AddModelError(nameof(model.Amount), "المبلغ يجب أن يكون أكبر من صفر.");

        if (!ModelState.IsValid)
        {
            model.Name = cashBox.Name;
            return PartialView("_DepositForm", model);
        }


            cashBox.CurrentBalance = Math.Abs(cashBox.CurrentBalance) + Math.Abs(model.Amount); // لا سلب على الإطلاق
            cashBox.LastAction = LastActionName.Update;
            cashBox.UpdatedById = User.GetUserId();
            cashBox.UpdatedOn = DateTime.Now;
            _unitOfWork.CashBoxes.Update(cashBox);

            await _unitOfWork.CashTransactions.AddAsync(new CashTransaction
            {
                CashBoxId = cashBox.Id,
                Amount = Math.Abs(model.Amount), // دائماً موجب
                TransactionType = CashTransactionType.Deposit,
                Notes = model.Notes,
                Date = DateTime.Now
            });

            await _unitOfWork.SaveChangesAsync();

            var viewModel = cashBox.Adapt<CashBoxViewModel>();
            viewModel.LastAction = cashBox.LastAction;
            var branch = await _unitOfWork.Branches.GetById(cashBox.BranchId);
            if (branch != null) viewModel.BranchName = branch.Name;

            return PartialView("_Row", viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Withdraw(int id)
    {
        var cashBox = await _unitOfWork.CashBoxes.GetById(id);
        if (cashBox == null || cashBox.LastAction == LastActionName.Delete)
            return NotFound();
        var model = new CashBoxTransactionFormViewModel
        {
            Id = cashBox.Id,
            Name = cashBox.Name
        };
        return PartialView("_WithdrawForm", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Withdraw(CashBoxTransactionFormViewModel model)
    {
        var cashBox = await _unitOfWork.CashBoxes.GetById(model.Id);
        if (cashBox == null || cashBox.LastAction == LastActionName.Delete)
            return NotFound();

        model.Name = cashBox.Name;
        if (model.Amount <= 0)
            ModelState.AddModelError(nameof(model.Amount), "المبلغ يجب أن يكون أكبر من صفر.");

        if (!ModelState.IsValid)
        {
            model.Name = cashBox.Name;
            return PartialView("_WithdrawForm", model);
        }

        decimal safeAmount = Math.Abs(model.Amount);
        decimal currentSafe = Math.Abs(cashBox.CurrentBalance);

        // التحقق من الرصيد قبل بدء الـ transaction
        if (safeAmount > currentSafe)
        {
            ModelState.AddModelError(nameof(model.Amount), _localization["InsufficientBalance"] + $" الرصيد الحالي: {currentSafe:0.00}");
            model.Name = cashBox.Name;
            return PartialView("_WithdrawForm", model);
        }
        cashBox.CurrentBalance -= model.Amount;

            cashBox.CurrentBalance = currentSafe - safeAmount; // النتيجة >= 0 بسبب الشرط أعلاه
            cashBox.LastAction = LastActionName.Update;
            cashBox.UpdatedById = User.GetUserId();
            cashBox.UpdatedOn = DateTime.Now;
            _unitOfWork.CashBoxes.Update(cashBox);

            await _unitOfWork.CashTransactions.AddAsync(new CashTransaction
            {
                CashBoxId = cashBox.Id,
                Amount = safeAmount, // دائماً موجب
                TransactionType = CashTransactionType.Withdraw,
                Notes = model.Notes,
                Date = DateTime.Now
            });

            await _unitOfWork.SaveChangesAsync();

            var viewModel = cashBox.Adapt<CashBoxViewModel>();
            viewModel.LastAction = cashBox.LastAction;
            var branch = await _unitOfWork.Branches.GetById(cashBox.BranchId);
            if (branch != null) viewModel.BranchName = branch.Name;

            return PartialView("_Row", viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var cashBox = await _unitOfWork.CashBoxes.GetById(id);
        if (cashBox == null)
            return NotFound();
        if (cashBox.CurrentBalance != 0)
            return BadRequest("لا يمكن حذف الخزنه إلا إذا كان الرصيد الحالي يساوي صفر.");


            cashBox.LastAction = LastActionName.Delete;
            cashBox.UpdatedById = User.GetUserId();
            cashBox.UpdatedOn = DateTime.Now;
            _unitOfWork.CashBoxes.Update(cashBox);
            await _unitOfWork.SaveChangesAsync();

            var viewModel = cashBox.Adapt<CashBoxViewModel>();
            viewModel.LastAction = cashBox.LastAction;
            var branch = await _unitOfWork.Branches.GetById(cashBox.BranchId);
            if (branch != null) viewModel.BranchName = branch.Name;

            return PartialView("_Row", viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Restore(int id)
    {
        var cashBox = await _unitOfWork.CashBoxes.GetById(id);
        if (cashBox == null) return NotFound();


            cashBox.LastAction = LastActionName.Update;
            cashBox.UpdatedById = User.GetUserId();
            cashBox.UpdatedOn = DateTime.Now;
            _unitOfWork.CashBoxes.Update(cashBox);
            await _unitOfWork.SaveChangesAsync();

            var viewModel = cashBox.Adapt<CashBoxViewModel>();
            viewModel.LastAction = cashBox.LastAction;
            var branch = await _unitOfWork.Branches.GetById(cashBox.BranchId);
            if (branch != null) viewModel.BranchName = branch.Name;

            return PartialView("_Row", viewModel);

    }
    [HttpGet]
    public async Task<IActionResult> Statement(int? cashBoxId, DateTime? dateFrom, DateTime? dateTo)
    {
        // لو المستخدم مختارش تاريخ، هنحدد آخر شهر تلقائيًا
        dateFrom ??= DateTime.Today.AddMonths(-1);
        dateTo ??= DateTime.Today;

        var model = new CashBoxStatementViewModel
        {
            CashBoxId = cashBoxId ?? 0,
            DateFrom = dateFrom,
            DateTo = dateTo,
            CashBoxes = GetCashBoxesList()
        };

        if (cashBoxId.HasValue && cashBoxId > 0 && dateFrom.HasValue && dateTo.HasValue)
        {

            var cashBox = await _unitOfWork.CashBoxes.GetById(cashBoxId.Value);
            if (cashBox != null)
            {
                model.CashBoxName = cashBox.Name;
                var branch = await _unitOfWork.Branches.GetById(cashBox.BranchId);
                model.BranchName = branch?.Name;

                // ضبط بداية ونهاية اليوم لضمان شمول اليوم بالكامل (حتى 23:59:59)
                var startDateTime = dateFrom.Value.Date;
                var endDateTime = dateTo.Value.Date.AddDays(1).AddTicks(-1);

                // 1) حساب الرصيد ما قبل بدء الفترة
                // تشمل الرصيد الافتتاحي الأساسي للخزنة + صافي كافة الحركات السابقة لتاريخ startDateTime
                var transactionsBeforePeriod = await _unitOfWork.CashTransactions
                    .GetTableNoTracking()
                    .Where(t => t.CashBoxId == cashBoxId.Value && t.Date < startDateTime)
                    .ToListAsync();

                bool hasAnyOpeningBalanceTx = await _unitOfWork.CashTransactions
                    .GetTableNoTracking()
                    .AnyAsync(t => t.CashBoxId == cashBoxId.Value && t.TransactionType == CashTransactionType.OpeningBalance);

                decimal totalInBefore = transactionsBeforePeriod
                    .Where(t => t.TransactionType == CashTransactionType.OpeningBalance
                             || t.TransactionType == CashTransactionType.Deposit
                             || t.TransactionType == CashTransactionType.Sales)
                    .Sum(t => Math.Abs(t.Amount));

                decimal totalOutBefore = transactionsBeforePeriod
                    .Where(t => t.TransactionType == CashTransactionType.Withdraw)
                    .Sum(t => Math.Abs(t.Amount));

                decimal baseOpening = hasAnyOpeningBalanceTx ? 0 : Math.Abs(cashBox.OpeningBalance);
                model.OpeningBalanceBeforePeriod = baseOpening + totalInBefore - totalOutBefore;

                // 2) جلب الحركات داخل الفترة الزمنية المحددة
                var transactionsInPeriod = await _unitOfWork.CashTransactions
                    .GetTableNoTracking()
                    .Where(t => t.CashBoxId == cashBoxId.Value
                             && t.Date >= startDateTime
                             && t.Date <= endDateTime)
                    .OrderBy(t => t.Date)
                    .ThenBy(t => t.Id)
                    .ToListAsync();

                model.HasTransactions = transactionsInPeriod.Any();

                var referenceNumbers = transactionsInPeriod
                    .Where(t => !string.IsNullOrEmpty(t.ReferenceNumber))
                    .Select(t => t.ReferenceNumber!)
                    .Distinct()
                    .ToList();

                var salesInvoicesByNumber = await _unitOfWork.SalesInvoices
                    .GetTableNoTracking()
                    .Where(i => referenceNumbers.Contains(i.InvoiceNumber))
                    .Select(i => new { i.InvoiceNumber, i.Id })
                    .ToDictionaryAsync(x => x.InvoiceNumber, x => x.Id);

                var purchaseInvoicesByNumber = await _unitOfWork.PurchaseInvoices
                    .GetTableNoTracking()
                    .Where(i => referenceNumbers.Contains(i.InvoiceNumber))
                    .Select(i => new { i.InvoiceNumber, i.Id })
                    .ToDictionaryAsync(x => x.InvoiceNumber, x => x.Id);

                // 3) بناء سطور كشف الحساب وتتبع الرصيد المتراكم
                model.Lines = BuildCashStatementLines(
                    transactionsInPeriod,
                    model.OpeningBalanceBeforePeriod,
                    salesInvoicesByNumber,
                    purchaseInvoicesByNumber);

                // 4) حساب الإجماليات للفترة والرصيد الختامي
                model.TotalIn = model.Lines.Sum(l => l.InAmount ?? 0);
                model.TotalOut = model.Lines.Sum(l => l.OutAmount ?? 0);
                model.ClosingBalanceAfterPeriod = model.Lines.LastOrDefault()?.BalanceAfter ?? model.OpeningBalanceBeforePeriod;
            }
        }

        return View(model);
    }

    private static List<CashStatementLineViewModel> BuildCashStatementLines(
        List<CashTransaction> txs,
        decimal balanceBeforePeriodStart,
        Dictionary<string, int> salesInvoicesByNumber,
        Dictionary<string, int> purchaseInvoicesByNumber)
    {
        var lines = new List<CashStatementLineViewModel>();
        decimal running = Math.Abs(balanceBeforePeriodStart);

        foreach (var t in txs)
        {
            bool isSalesInvoice = t.TransactionType == CashTransactionType.Sales
                && !string.IsNullOrEmpty(t.ReferenceNumber)
                && salesInvoicesByNumber.TryGetValue(t.ReferenceNumber!, out _);

            bool isPurchaseInvoice = t.TransactionType == CashTransactionType.Withdraw
                && !string.IsNullOrEmpty(t.ReferenceNumber)
                && purchaseInvoicesByNumber.TryGetValue(t.ReferenceNumber!, out _);

            var line = new CashStatementLineViewModel
            {
                Date = t.Date,
                TransactionType = t.TransactionType,
                TransactionTypeName = GetTransactionTypeName(t.TransactionType, isPurchaseInvoice),
                Notes = string.IsNullOrWhiteSpace(t.Notes) ? GetTransactionTypeName(t.TransactionType, isPurchaseInvoice) : t.Notes,
                ReferenceNumber = t.ReferenceNumber,
                BalanceBefore = running,
                IsSalesInvoice = isSalesInvoice,
                IsPurchaseInvoice = isPurchaseInvoice,
                ReferenceInvoiceId = isSalesInvoice && !string.IsNullOrEmpty(t.ReferenceNumber) ? salesInvoicesByNumber[t.ReferenceNumber!] :
                    isPurchaseInvoice && !string.IsNullOrEmpty(t.ReferenceNumber) ? purchaseInvoicesByNumber[t.ReferenceNumber!] : null
            };

            decimal safeAmount = Math.Abs(t.Amount);

            if (t.TransactionType == CashTransactionType.OpeningBalance
                || t.TransactionType == CashTransactionType.Deposit
                || t.TransactionType == CashTransactionType.Sales)
            {
                line.InAmount = safeAmount;
                running += safeAmount;
            }
            else if (t.TransactionType == CashTransactionType.Withdraw)
            {
                line.OutAmount = safeAmount;
                running = Math.Max(0, running - safeAmount);
            }

            line.BalanceAfter = running;

            lines.Add(line);
        }

        return lines;
    }

    private IEnumerable<SelectListItem> GetCashBoxesList()
    {
        return _unitOfWork.CashBoxes
            .AsQueryable()
            .Include(c => c.Branch)
            .Where(c => c.LastAction != LastActionName.Delete)
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Branch != null ? $"{c.Name} - {c.Branch.Name}" : c.Name
            })
            .ToList();
    }

    private static string GetTransactionTypeName(CashTransactionType type, bool isPurchaseInvoice)
    {
        return type switch
        {
            CashTransactionType.Deposit => "إيداع نقدي",
            CashTransactionType.Withdraw => isPurchaseInvoice ? "مشتريات كاش" : "سحب نقدي",
            CashTransactionType.Sales => "مبيعات كاش",
            CashTransactionType.OpeningBalance => "رصيد افتتاحي",
            _ => type.ToString()
        };
    }
}
