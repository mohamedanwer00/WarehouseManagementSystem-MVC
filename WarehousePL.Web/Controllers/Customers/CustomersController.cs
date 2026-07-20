using WarehouseBLL.BusinessServices.View_Models.Customer;
using WarehouseBLL.FormViewModels.Customer;

namespace WarehousePL.Web.Controllers.Customers
{
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

            customer.CurrentBalance = model.OpeningBalanceType == BalanceType.Debitor
                ? -model.OpeningBalance
                : model.OpeningBalance;

            await _unitOfWork.Customers.AddAsync(customer);
            _unitOfWork.SaveChanges();

            var viewModel = customer.Adapt<CustomerViewModel>();
            //return RedirectToAction(nameof(Index));
            return PartialView("_Row", viewModel);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var customer = _unitOfWork.Customers.GetById(id);
            if (customer is null)
                return NotFound();

            var model = customer.Adapt<CustomerFormViewModel>();
            return PartialView("_Form", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CustomerFormViewModel model)
        {
            // فحص تكرار الاسم مع استثناء العميل الحالي نفسه عند التعديل
            if (_unitOfWork.Customers.GetTableNoTracking().Any(x => x.Name == model.Name && x.Id != model.Id))
            {
                ModelState.AddModelError(nameof(model.Name), "اسم العميل موجود بالفعل");
            }

            // فحص تكرار رقم الهاتف مع استثناء العميل الحالي نفسه عند التعديل
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

            // تثبيت الإعدادات المالية القديمة لمنع التصفير والتلاعب
            customer.OpeningBalance = oldOpeningBalance;
            customer.OpeningBalanceType = oldOpeningBalanceType;
            customer.CurrentBalance = oldCurrentBalance;

            customer.LastAction = LastActionName.Update;
            customer.UpdatedById = User.GetUserId();
            customer.UpdatedOn = DateTime.Now;

            _unitOfWork.Customers.Update(customer);
            _unitOfWork.SaveChanges();

            var viewModel = customer.Adapt<CustomerViewModel>();
            //return RedirectToAction(nameof(Index));
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
            _unitOfWork.SaveChanges();

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
            _unitOfWork.SaveChanges();

            var ViewModel = customer.Adapt<CustomerViewModel>();
            ViewModel.LastAction = customer.LastAction;
            return PartialView("_Row", ViewModel);
        }
    }
}
