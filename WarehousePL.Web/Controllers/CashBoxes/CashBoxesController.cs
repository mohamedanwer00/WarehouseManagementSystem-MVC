using Microsoft.AspNetCore.Mvc;
using WarehouseBLL.BusinessServices.View_Models.CashBox;
using WarehouseBLL.FormViewModels.CashBox;
using WarehouseDAL.Entities;

namespace WarehousePL.Web.Controllers.CashBoxes
{
    public class CashBoxesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CashBoxesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
                ModelState.AddModelError(nameof(model.Name), "اسم الخزنة موجود بالفعل.");
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

            //  الرصيد الحالي يساوي الرصيد الافتتاحي  
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

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var cashBox = _unitOfWork.CashBoxes.GetById(id);
            if (cashBox == null)
            {
                return NotFound();
            }
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
            if (cashBox == null)
            {
                return NotFound();
            }
            // التحقق من عدم تكرار الاسم
            var isNameExists = _unitOfWork.CashBoxes
                .GetAll()
                .Any(c => c.Id != model.Id && c.Name.Trim().ToLower() == model.Name.Trim().ToLower() && !c.IsDeleted);
            if (isNameExists)
            {
                ModelState.AddModelError(nameof(model.Name), "اسم الخزنة موجود بالفعل.");
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
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Deposit(int id)
        {
            var cashBox = _unitOfWork.CashBoxes.GetById(id);
            if (cashBox == null || cashBox.IsDeleted)
                return NotFound();

            var model = new CashBoxTransactionFormViewModel
            {
                CashBoxId = cashBox.Id,
                CashBoxName = cashBox.Name
            };
            return PartialView("_DepositForm", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Deposit(CashBoxTransactionFormViewModel model)
        {
            var cashBox = _unitOfWork.CashBoxes.GetById(model.CashBoxId);
            if (cashBox == null || cashBox.IsDeleted) return NotFound();

            if (!ModelState.IsValid)
            {
                model.CashBoxName = cashBox.Name;
                return PartialView("_DepositForm", model);
            }

            cashBox.CurrentBalance += model.Amount;
            cashBox.LastAction = LastActionName.Update;
            _unitOfWork.CashBoxes.Update(cashBox);
            _unitOfWork.SaveChanges();

            // ✨ الحل: تحويل الـ Entity لـ ViewModel وإرجاع الـ Row ليحدث نفسه تلقائياً
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
                CashBoxId = cashBox.Id,
                CashBoxName = cashBox.Name
            };
            return PartialView("_WithdrawForm", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Withdraw(CashBoxTransactionFormViewModel model)
        {
            var cashBox = _unitOfWork.CashBoxes.GetById(model.CashBoxId);
            if (cashBox == null || cashBox.IsDeleted) return NotFound();

            if (model.Amount > cashBox.CurrentBalance)
            {
                ModelState.AddModelError(nameof(model.Amount), "المبلغ أكبر من الرصيد الحالي.");
            }

            if (!ModelState.IsValid)
            {
                model.CashBoxName = cashBox.Name;
                return PartialView("_WithdrawForm", model);
            }

            cashBox.CurrentBalance -= model.Amount;
            cashBox.LastAction = LastActionName.Update;
            _unitOfWork.CashBoxes.Update(cashBox);
            _unitOfWork.SaveChanges();

            // ✨ الحل: تحويل الـ Entity لـ ViewModel وإرجاع الـ Row ليحدث نفسه تلقائياً
            var viewModel = cashBox.Adapt<CashBoxViewModel>();
            viewModel.LastAction = cashBox.LastAction;

            var branch = _unitOfWork.Branches.GetById(cashBox.BranchId);
            if (branch != null) viewModel.BranchName = branch.Name;

            return PartialView("_Row", viewModel);
        }
    }
}
