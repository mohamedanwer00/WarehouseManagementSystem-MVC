
using WarehouseBLL.BusinessServices.View_Models.CashBox;
using WarehouseBLL.FormViewModels.CashBox;
using WarehouseBLL.Const;
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

        public IActionResult Index()
        {
            var cashBoxes = _unitOfWork.CashBoxes.GetAll();

            foreach (var cb in cashBoxes)
            {
                if (cb.Branch == null)
                    cb.Branch = _unitOfWork.Branches.GetById(cb.BranchId);
            }

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
        public IActionResult Create(CashBoxFormViewModel model)
        {
            var isNameExists = _unitOfWork.CashBoxes
                .GetAll()
                .Any(c => c.Name.Trim().ToLower() == model.Name.Trim().ToLower() && !c.IsDeleted);

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

            _unitOfWork.CashBoxes.Add(cashBox);
            _unitOfWork.SaveChanges();

            var viewModel = cashBox.Adapt<CashBoxViewModel>();
            viewModel.LastAction = cashBox.LastAction;

            var selectedBranch = _unitOfWork.Branches.GetById(model.SelectedBranch);
            if (selectedBranch != null)
            {
                viewModel.BranchName = selectedBranch.Name;
            }

            return PartialView("_Row", viewModel);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var cashBox = _unitOfWork.CashBoxes.GetById(id);
            if (cashBox == null) return NotFound();

            var branches = _unitOfWork.Branches.GetAll();
            var viewModel = cashBox.Adapt<CashBoxFormViewModel>();
            viewModel.Branches = branches.Select(b => new SelectListItem
            {
                Text = b.Name,
                Value = b.Id.ToString()
            });
            return PartialView("_Form", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CashBoxFormViewModel model)
        {
            var cashBox = _unitOfWork.CashBoxes.GetById(model.Id.Value);
            if (cashBox == null) return NotFound();

            var isNameExists = _unitOfWork.CashBoxes
                .GetAll()
                .Any(c => c.Id != model.Id && c.Name.Trim().ToLower() == model.Name.Trim().ToLower() && !c.IsDeleted);

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

            _unitOfWork.CashBoxes.Update(cashBox);
            _unitOfWork.SaveChanges();

            var viewModel = cashBox.Adapt<CashBoxViewModel>();
            viewModel.LastAction = cashBox.LastAction;

            var selectedBranch = _unitOfWork.Branches.GetById(model.SelectedBranch);
            if (selectedBranch != null)
            {
                viewModel.BranchName = selectedBranch.Name;
            }

            return PartialView("_Row", viewModel);
        }

        [HttpGet]
        public IActionResult Deposit(int id)
        {
            var cashBox = _unitOfWork.CashBoxes.GetById(id);
            if (cashBox == null || cashBox.IsDeleted) return NotFound();

            var model = new CashBoxTransactionFormViewModel
            {
                Id = cashBox.Id,
                Name = cashBox.Name
            };
            return PartialView("_DepositForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Deposit(CashBoxTransactionFormViewModel model)
        {
            var cashBox = _unitOfWork.CashBoxes.GetById(model.Id);
            if (cashBox == null || cashBox.IsDeleted) return NotFound();
            model.Name = cashBox.Name;

            if (!ModelState.IsValid)
            {
                model.Name = cashBox.Name;
                return PartialView("_DepositForm", model);
            }

            cashBox.CurrentBalance += model.Amount;
            cashBox.LastAction = LastActionName.Update;
            _unitOfWork.CashBoxes.Update(cashBox);
            _unitOfWork.SaveChanges();

            var viewModel = cashBox.Adapt<CashBoxViewModel>();
            viewModel.LastAction = cashBox.LastAction;

            var branch = _unitOfWork.Branches.GetById(cashBox.BranchId);
            if (branch != null) viewModel.BranchName = branch.Name;

            return PartialView("_Row", viewModel);
        }

        [HttpGet]
        public IActionResult Withdraw(int id)
        {
            var cashBox = _unitOfWork.CashBoxes.GetById(id);
            if (cashBox == null || cashBox.IsDeleted) 
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
        public IActionResult Withdraw(CashBoxTransactionFormViewModel model)
        {
            var cashBox = _unitOfWork.CashBoxes.GetById(model.Id);
            if (cashBox == null || cashBox.IsDeleted) return NotFound();
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
            _unitOfWork.CashBoxes.Update(cashBox);
            _unitOfWork.SaveChanges();
            var viewModel = cashBox.Adapt<CashBoxViewModel>();
            viewModel.LastAction = cashBox.LastAction;
            var branch = _unitOfWork.Branches.GetById(cashBox.BranchId);
            if (branch != null) viewModel.BranchName = branch.Name;
            return PartialView("_Row", viewModel);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var cashBox = _unitOfWork.CashBoxes.GetById(id);
            if (cashBox == null) return NotFound();

            cashBox.IsDeleted = true;
            cashBox.LastAction = LastActionName.Delete;
            _unitOfWork.CashBoxes.Update(cashBox);
            _unitOfWork.SaveChanges();

            var viewModel = cashBox.Adapt<CashBoxViewModel>();
            viewModel.LastAction = cashBox.LastAction;
            var branch = _unitOfWork.Branches.GetById(cashBox.BranchId);
            if (branch != null) viewModel.BranchName = branch.Name;

            return PartialView("_Row", viewModel);
        }

        [HttpPost]
        public IActionResult Restore(int id)
        {
            var cashBox = _unitOfWork.CashBoxes.GetById(id);
            if (cashBox == null) return NotFound();

            cashBox.IsDeleted = false;
            cashBox.LastAction = LastActionName.Update;
            _unitOfWork.CashBoxes.Update(cashBox);
            _unitOfWork.SaveChanges();

            var viewModel = cashBox.Adapt<CashBoxViewModel>();
            viewModel.LastAction = cashBox.LastAction;
            var branch = _unitOfWork.Branches.GetById(cashBox.BranchId);
            if (branch != null) viewModel.BranchName = branch.Name;

            return PartialView("_Row", viewModel);
        }
    }
}