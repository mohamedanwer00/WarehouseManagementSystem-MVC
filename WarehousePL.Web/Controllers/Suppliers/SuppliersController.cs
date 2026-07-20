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

        public IActionResult Index()
        {
            var suppliers = _unitOfWork.Suppliers.GetTableNoTracking().ToList();
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
            if (_unitOfWork.Suppliers.GetTableNoTracking()
                .Any(x => x.Name == model.Name && x.LastAction != LastActionName.Delete))
                ModelState.AddModelError(nameof(model.Name), "اسم المورد موجود بالفعل");

            if (_unitOfWork.Suppliers.GetTableNoTracking()
                .Any(x => x.PhoneNumber == model.PhoneNumber && x.LastAction != LastActionName.Delete))
                ModelState.AddModelError(nameof(model.PhoneNumber), "رقم الهاتف مستخدم بالفعل");

            if (!ModelState.IsValid)
                return PartialView("_Form", model);

            var supplier = model.Adapt<Supplier>();
            supplier.LastAction = LastActionName.Insert;
            supplier.CreatedById = User.GetUserId();
            supplier.CreatedOn = DateTime.Now;
            supplier.CurrentBalance = model.OpeningBalanceType == BalanceType.Creditor
                ? model.OpeningBalance
                : -model.OpeningBalance;

            await _unitOfWork.Suppliers.AddAsync(supplier);
            _unitOfWork.SaveChanges();

            var viewModel = supplier.Adapt<SupplierViewModel>();
            viewModel.LastAction = supplier.LastAction;
            return PartialView("_Row", viewModel);
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var supplier = _unitOfWork.Suppliers.GetById(id);
            if (supplier is null)
                return NotFound();

            var model = supplier.Adapt<SupplierFormViewModel>();
            return PartialView("_Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SupplierFormViewModel model)
        {
            if (_unitOfWork.Suppliers.GetTableNoTracking()
                .Any(x => x.Name == model.Name && x.Id != model.Id && x.LastAction != LastActionName.Delete))
                ModelState.AddModelError(nameof(model.Name), "اسم المورد موجود بالفعل");

            if (_unitOfWork.Suppliers.GetTableNoTracking()
                .Any(x => x.PhoneNumber == model.PhoneNumber && x.Id != model.Id && x.LastAction != LastActionName.Delete))
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
            _unitOfWork.SaveChanges();

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
            _unitOfWork.SaveChanges();

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
            _unitOfWork.SaveChanges();

            var rowViewModel = supplier.Adapt<SupplierViewModel>();
            rowViewModel.LastAction = supplier.LastAction;
            return PartialView("_Row", rowViewModel);
        }
    }
}
