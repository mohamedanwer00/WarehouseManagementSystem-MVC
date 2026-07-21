using WarehouseBLL.BusinessServices.View_Models;
using WarehouseBLL.BusinessServices.View_Models.Branch;
using WarehouseBLL.Extensions;
using WarehouseBLL.FormViewModels.Branch;
using WarehouseDAL.Entities;
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
        public async Task<IActionResult> Index()
        {
            var viewModel = await _unitOfWork.Branches.AsQueryable()
                .ProjectToType<BranchViewModel>()
                .ToListAsync();

            return View(viewModel);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return PartialView("_Form");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BranchViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_Form", model);
            var branch = model.Adapt<Branch>();
            branch.LastAction = LastActionName.Insert;
            branch.CreatedById=User.GetUserId();
            branch.CreatedOn = DateTime.Now;
            await _unitOfWork.Branches.AddAsync(branch);
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
        public async Task<IActionResult> Edit(BranchFormViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_Form", model);
            var branch = await _unitOfWork.Branches.GetById(model.Id!.Value);
            if (branch is null)
                return NotFound();
            branch = model.Adapt(branch);
            branch.LastAction = LastActionName.Update;
            branch.UpdatedById=User.GetUserId();
            branch.UpdatedOn=DateTime.Now;
            _unitOfWork.Branches.Update(branch);
            _unitOfWork.SaveChanges();
            var viewModel = branch.Adapt<BranchViewModel>();
            viewModel.LastAction = branch.LastAction;
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var branch = await _unitOfWork.Branches.GetById(id);

            if (branch is null)
                return NotFound();

            branch.LastAction = LastActionName.Delete;
            branch.UpdatedById = User.GetUserId();
            branch.UpdatedOn = DateTime.Now;
            _unitOfWork.Branches.Update(branch);
            _unitOfWork.SaveChanges();

            var viewModel = branch.Adapt<BranchViewModel>();
            viewModel.LastAction = branch.LastAction;

            return PartialView("_Row", viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id)
        {
            var branch = await _unitOfWork.Branches.GetById(id);

            if (branch is null)
                return NotFound();

            branch.LastAction = LastActionName.Update;
            branch.UpdatedById = User.GetUserId();
            branch.UpdatedOn = DateTime.Now;
            _unitOfWork.Branches.Update(branch);
            _unitOfWork.SaveChanges();

            var viewModel = branch.Adapt<BranchViewModel>();
            viewModel.LastAction = branch.LastAction;

            return PartialView("_Row", viewModel);
        }
    }
}
