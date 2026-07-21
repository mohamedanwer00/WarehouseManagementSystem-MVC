
using WarehouseBLL.BusinessServices.View_Models.CashBox;
using WarehouseBLL.Const;
using WarehouseBLL.Extensions;
using WarehouseBLL.FormViewModels.CashBox;
using WarehouseDAL.Entities;

namespace WarehousePL.Web.Controllers.CashBoxes
{
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
            cashBox.CurrentBalance = model.OpeningBalance;
            cashBox.LastAction = LastActionName.Insert;
            cashBox.CreatedById = User.GetUserId();
            cashBox.CreatedOn = DateTime.Now;
            await _unitOfWork.CashBoxes.AddAsync(cashBox);
            _unitOfWork.SaveChanges();

            var viewModel = cashBox.Adapt<CashBoxViewModel>();
            viewModel.LastAction = cashBox.LastAction;

            var selectedBranch = await _unitOfWork.Branches.GetById(model.SelectedBranch);
            if (selectedBranch != null)
            {
                viewModel.BranchName = selectedBranch.Name;
            }

            //return PartialView("_Row", viewModel);
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
            cashBox.OpeningBalance = model.OpeningBalance;
            cashBox.LastAction = LastActionName.Update;
            cashBox.UpdatedById=User.GetUserId();
            cashBox.UpdatedOn= DateTime.Now;
            _unitOfWork.CashBoxes.Update(cashBox);
            _unitOfWork.SaveChanges();

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

            if (!ModelState.IsValid)
            {
                model.Name = cashBox.Name;
                return PartialView("_DepositForm", model);
            }

            cashBox.CurrentBalance += model.Amount;
            cashBox.LastAction = LastActionName.Update;
            cashBox.UpdatedById = User.GetUserId();
            cashBox.UpdatedOn = DateTime.Now;
            _unitOfWork.CashBoxes.Update(cashBox);
            _unitOfWork.SaveChanges();

            var viewModel = cashBox.Adapt<CashBoxViewModel>();
            viewModel.LastAction = cashBox.LastAction;

            var branch = await _unitOfWork.Branches.GetById(cashBox.BranchId);
            if (branch != null) viewModel.BranchName = branch.Name;

            return PartialView("_Row", viewModel);
        }

        [HttpGet]
        public IActionResult Withdraw(int id)
        {
            var cashBox = _unitOfWork.CashBoxes.Adapt<CashBoxViewModel>();
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
            if (!ModelState.IsValid)
            {
                model.Name = cashBox.Name;
                return PartialView("_WithdrawForm", model);
            }
            if (model.Amount > cashBox.CurrentBalance)
            {
                ModelState.AddModelError(nameof(model.Amount), _localization["InsufficientBalance"]);
                model.Name = cashBox.Name;
                return PartialView("_WithdrawForm", model);
            }
            cashBox.CurrentBalance -= model.Amount;
            cashBox.LastAction = LastActionName.Update;
            cashBox.UpdatedById = User.GetUserId();
            cashBox.UpdatedOn = DateTime.Now;
            _unitOfWork.CashBoxes.Update(cashBox);
            _unitOfWork.SaveChanges();
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
            _unitOfWork.SaveChanges();

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
            _unitOfWork.SaveChanges();

            var viewModel = cashBox.Adapt<CashBoxViewModel>();
            viewModel.LastAction = cashBox.LastAction;
            var branch = await _unitOfWork.Branches.GetById(cashBox.BranchId);
            if (branch != null) viewModel.BranchName = branch.Name;

            return PartialView("_Row", viewModel);
        }
    }
}