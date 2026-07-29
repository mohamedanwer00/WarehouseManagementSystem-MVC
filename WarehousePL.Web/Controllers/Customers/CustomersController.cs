using WarehouseBLL.BusinessServices.View_Models.Customer;
using WarehouseBLL.BusinessServices.View_Models.Supplier;
using WarehouseBLL.FormViewModels.Customer;
using WarehouseDAL.Entities.Enums;
using WarehouseDAL.Entities.Transactions;
using Microsoft.EntityFrameworkCore;
namespace WarehousePL.Web.Controllers.Customers;

public class CustomersController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public CustomersController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        var customers = _unitOfWork.Customers.GetTableNoTracking().ToList();
        var viewModel = customers.Adapt<List<CustomerViewModel>>();
        return View(viewModel);
    }


    [HttpGet]
    public IActionResult Create()
    {
        var model = new CustomerFormViewModel();
        return PartialView("_Form", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CustomerFormViewModel model)
    {
        if (_unitOfWork.Customers.GetTableNoTracking().Any(x => x.Name == model.Name))
        {
            ModelState.AddModelError(nameof(model.Name), "اسم العميل موجود بالفعل");
        }

        if (_unitOfWork.Customers.GetTableNoTracking().Any(x => x.PhoneNumber == model.PhoneNumber))
        {
            ModelState.AddModelError(nameof(model.PhoneNumber), "رقم الهاتف مستخدم بالفعل");
        }

        if (!ModelState.IsValid)
            return PartialView("_Form", model);

            var customer = model.Adapt<Customer>();
            customer.LastAction = LastActionName.Insert;
            customer.CreatedById = User.GetUserId();
            customer.CreatedOn = DateTime.Now;

            customer.CurrentBalance = model.OpeningBalance;

            await _unitOfWork.Customers.AddAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            if (model.OpeningBalance > 0)
            {
                await _unitOfWork.CustomerTransactions.AddAsync(new CustomerTransaction
                {
                    CustomerId = customer.Id,
                    Amount = Math.Abs(model.OpeningBalance),
                    CustomerTransactionType = CustomerTransactionType.OPenBalance,
                    Notes = "رصيد افتتاحي",
                    Date = DateTime.Now
                });
                await _unitOfWork.SaveChangesAsync();
            }


            var viewModel = customer.Adapt<CustomerViewModel>();
            return PartialView("_Row", viewModel);
    }

    [HttpGet]
    public async Task< IActionResult> Edit(int id)
    {
        Customer? customer = await _unitOfWork.Customers.GetById(id);
        if (customer is null)
            return NotFound();

        CustomerFormViewModel model = customer.Adapt<CustomerFormViewModel>();
        return PartialView("_Form", model);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CustomerFormViewModel model)
    {
        if (_unitOfWork.Customers.GetTableNoTracking().Any(x => x.Name == model.Name && x.Id != model.Id))
        {
            ModelState.AddModelError(nameof(model.Name), "اسم العميل موجود بالفعل");
        }

        if (_unitOfWork.Customers.GetTableNoTracking().Any(x => x.PhoneNumber == model.PhoneNumber && x.Id != model.Id))
        {
            ModelState.AddModelError(nameof(model.PhoneNumber), "رقم الهاتف مستخدم بالفعل");
        }

        if (!ModelState.IsValid)
            return PartialView("_Form", model);

        var customer = await _unitOfWork.Customers.GetById(model.Id.Value);
        if (customer == null)
            return NotFound();

            var oldOpeningBalance = customer.OpeningBalance;
            var oldOpeningBalanceType = customer.OpeningBalanceType;
            var oldCurrentBalance = customer.CurrentBalance;

            model.Adapt(customer);

            customer.OpeningBalance = oldOpeningBalance;
            customer.OpeningBalanceType = oldOpeningBalanceType;
            customer.CurrentBalance = oldCurrentBalance;

            customer.LastAction = LastActionName.Update;
            customer.UpdatedById = User.GetUserId();
            customer.UpdatedOn = DateTime.Now;

            _unitOfWork.Customers.Update(customer);
            await _unitOfWork.SaveChangesAsync();

            var viewModel = customer.Adapt<CustomerViewModel>();
            return PartialView("_Row", viewModel);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]

    public async Task<IActionResult> Delete(int id)
    {
        var customer = await _unitOfWork.Customers.GetById(id);

        if (customer is null)
            return NotFound();

        if (customer.CurrentBalance != 0)
            return BadRequest("لا يمكن حذف العميل إلا إذا كان الرصيد الحالي يساوي صفر.");


            customer.LastAction = LastActionName.Delete;
            customer.UpdatedById = User.GetUserId();
            customer.UpdatedOn = DateTime.Now;

            _unitOfWork.Customers.Update(customer);
            await _unitOfWork.SaveChangesAsync();

            var viewModel = customer.Adapt<CustomerViewModel>();

            return PartialView("_Row", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]

    public async Task<IActionResult> Restore(int id)
    {
        var customer = await _unitOfWork.Customers.GetById(id);
        if (customer is null)
            return NotFound();

            customer.LastAction = LastActionName.Update;
            customer.UpdatedById = User.GetUserId();
            customer.UpdatedOn = DateTime.Now;
            _unitOfWork.Customers.Update(customer);
            await _unitOfWork.SaveChangesAsync();

            var ViewModel = customer.Adapt<CustomerViewModel>();
            ViewModel.LastAction = customer.LastAction;
            return PartialView("_Row", ViewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Statement(int? customerId, DateTime? dateFrom, DateTime? dateTo)
    {
        dateFrom ??= DateTime.Today.AddMonths(-1);
        dateTo ??= DateTime.Today;

        var model = new CustomerStatementViewModel
        {
            CustomerId = customerId ?? 0,
            DateFrom = dateFrom,
            DateTo = dateTo,
            Customers = GetCustomersList()
        };

        if (customerId.HasValue && customerId > 0)
        {
            var customer = await _unitOfWork.Customers.GetById(customerId.Value);
            if (customer != null)
            {
                model.CustomerName = customer.Name;
                model.OpeningBalance = customer.OpeningBalance;
                model.OpeningBalanceType = customer.OpeningBalanceType;

                DateTime searchDateFrom = dateFrom.Value.Date;
                DateTime searchDateTo = dateTo.Value.Date.AddDays(1).AddTicks(-1);

                var priorTransactions = await _unitOfWork.CustomerTransactions
                    .GetTableNoTracking()
                    .Where(t => t.CustomerId == customerId.Value && t.Date < searchDateFrom)
                    .ToListAsync();

                decimal initialBalance = customer.OpeningBalanceType == BalanceType.Debitor
                    ? customer.OpeningBalance
                    : -customer.OpeningBalance;

                foreach (var pt in priorTransactions)
                {
                    if (pt.CustomerTransactionType == CustomerTransactionType.SalesInvoice ||
                        pt.CustomerTransactionType == CustomerTransactionType.OPenBalance)
                    {
                        initialBalance += pt.Amount;
                    }
                    else if (pt.CustomerTransactionType == CustomerTransactionType.Payment)
                    {
                        initialBalance -= pt.Amount;
                    }
                }

                // 2. جلب معاملات الفترة المحددة
                var transactions = await _unitOfWork.CustomerTransactions
                    .GetTableNoTracking()
                    .Where(t => t.CustomerId == customerId.Value && t.Date >= searchDateFrom && t.Date <= searchDateTo)
                    .Include(t => t.SalesInvoice)
                    .OrderBy(t => t.Date)
                    .ToListAsync();

                model.HasTransactions = transactions.Any();
                model.Lines = BuildStatementLines(transactions, initialBalance);

                model.TotalDebtor = model.Lines.Sum(l => l.DebtorAmount ?? 0);
                model.TotalCreditor = model.Lines.Sum(l => l.CreditAmount ?? 0);
                model.ClosingBalance = model.Lines.LastOrDefault()?.RunningBalance ?? initialBalance;
            }
        }

        return View(model);
    }

    private List<StatementLineViewModel> BuildStatementLines(
        List<CustomerTransaction> transactions,
        decimal initialBalance)
    {
        decimal running = initialBalance;
        var lines = new List<StatementLineViewModel>();

        foreach (var t in transactions)
        {
            string notes = t.Notes;

            if (t.CustomerTransactionType == CustomerTransactionType.SalesInvoice ||
                t.CustomerTransactionType == CustomerTransactionType.OPenBalance)
            {
                if (t.CustomerTransactionType == CustomerTransactionType.OPenBalance && string.IsNullOrWhiteSpace(notes))
                    notes = "رصيد افتتاحي";

                running += t.Amount;
                lines.Add(new StatementLineViewModel
                {
                    Date = t.Date,
                    Notes = notes,
                    DebtorAmount = t.Amount,
                    RunningBalance = running,
                    InvoiceId = t.SalesInvoiceId,
                    InvoiceNumber = t.SalesInvoice?.InvoiceNumber,
                    IsPurchaseInvoice = false
                });
            }
            else if (t.CustomerTransactionType == CustomerTransactionType.Payment)
            {
                if (string.IsNullOrWhiteSpace(notes))
                   

                running -= t.Amount;
                lines.Add(new StatementLineViewModel
                {
                    Date = t.Date,
                    Notes = notes,
                    CreditAmount = t.Amount,
                    RunningBalance = running,
                    InvoiceId = t.SalesInvoiceId,
                    InvoiceNumber = t.SalesInvoice?.InvoiceNumber,
                    IsPurchaseInvoice = false
                });
            }
        }

        return lines;
    }

    private IEnumerable<SelectListItem> GetCustomersList()
    {
        return _unitOfWork.Customers
            .GetAll(x => x.LastAction != LastActionName.Delete)
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name })
            .ToList();
    }


}