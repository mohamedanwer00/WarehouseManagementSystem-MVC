using WarehouseBLL.BusinessServices.View_Models;
using WarehouseBLL.BusinessServices.View_Models.Branch;
using WarehouseBLL.FormViewModels.Branch;
using WarehouseDAL.Entities.Entities;
using WarehouseDAL.Repositories.Implememtation;

namespace WarehousePL.Web.Controllers.Branches
{
    public class BranchesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public BranchesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var branches = _unitOfWork.Branches.GetAll();

            var viewModel = branches.Adapt<IEnumerable<BranchViewModel>>();

            //return View(viewModel);
            return View("~/Views/Branches/Index.cshtml", viewModel);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return PartialView("_Form");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BranchViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_Form", model);
            var branch = model.Adapt<Branch>();
            branch.LastAction = LastActionName.Insert;
            _unitOfWork.Branches.Add(branch);
            _unitOfWork.SaveChanges();
            var viewModel = branch.Adapt<BranchViewModel>();
            viewModel.LastAction = branch.LastAction;
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var branch = _unitOfWork.Branches.GetById(id);
            if (branch is null)
                return NotFound();
            var viewModel = branch.Adapt<BranchFormViewModel>();
            return PartialView("_Form", viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(BranchFormViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_Form", model);
            var branch = _unitOfWork.Branches.GetById(model.Id!.Value);
            if (branch is null)
                return NotFound();
            branch = model.Adapt(branch);
            branch.LastAction = LastActionName.Update;
            _unitOfWork.Branches.Update(branch);
            _unitOfWork.SaveChanges();
            var viewModel = branch.Adapt<BranchViewModel>();
            viewModel.LastAction = branch.LastAction;
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var branch = _unitOfWork.Branches.GetById(id);

            if (branch is null)
                return NotFound();

            branch.IsDeleted = true;
            branch.LastAction = LastActionName.Delete;

            _unitOfWork.Branches.Update(branch);
            _unitOfWork.SaveChanges();

            var viewModel = branch.Adapt<BranchViewModel>();
            viewModel.LastAction = branch.LastAction;

            return PartialView("_Row", viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Restore(int id)
        {
            var branch = _unitOfWork.Branches.GetById(id);

            if (branch is null)
                return NotFound();

            branch.IsDeleted = false;
            branch.LastAction = LastActionName.Update;

            _unitOfWork.Branches.Update(branch);
            _unitOfWork.SaveChanges();

            var viewModel = branch.Adapt<BranchViewModel>();
            viewModel.LastAction = branch.LastAction;

            return PartialView("_Row", viewModel);
        }
    }
}
