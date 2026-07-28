using WarehouseBLL.BusinessServices.View_Models.Warehouse;
using WarehouseBLL.Extensions;
using WarehouseBLL.FormViewModels.Warehouse;
using WarehouseDAL.Entities;

namespace WarehousePL.Web.Controllers.Warehouses
{
    public class WarehousesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public WarehousesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var warehouses = await _unitOfWork.Warehouses.AsQueryable()
                .Include(w => w.Branch)
                .ToListAsync();

            var viewModel = warehouses.Adapt<IEnumerable<WarehouseViewModel>>();
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var branches = _unitOfWork.Branches.GetAll();

            var viewModel = new WarehouseFormViewModel
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
        public async Task<IActionResult> Create(WarehouseFormViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Name))
            {
                model.Name = model.Name.Trim();
            }

            bool isNameExists = await _unitOfWork.Warehouses
                .AsQueryable()
                .AnyAsync(w => w.Name.Trim().ToLower() == model.Name.ToLower()
                            && w.LastAction != LastActionName.Delete);

            if (isNameExists)
            {
                ModelState.AddModelError(nameof(model.Name), "اسم المخزن موجود بالفعل.");
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

            var warehouse = model.Adapt<Warehouse>();

            warehouse.BranchId = model.SelectedBranch; 
            warehouse.Address ??= "Default Address"; 

            warehouse.LastAction = LastActionName.Insert;
            warehouse.CreatedById = User.GetUserId();
            warehouse.CreatedOn = DateTime.Now;

            await _unitOfWork.Warehouses.AddAsync(warehouse);
            await _unitOfWork.SaveChangesAsync();

            var viewModel = warehouse.Adapt<WarehouseViewModel>();
            viewModel.LastAction = warehouse.LastAction;

            var selectedBranch = await _unitOfWork.Branches.GetById(model.SelectedBranch);
            if (selectedBranch != null)
            {
                viewModel.BranchName = selectedBranch.Name;
            }

            return PartialView("_Row", viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Warehouse? warehouse = await _unitOfWork.Warehouses.GetById(id);
            if (warehouse is null)
                return NotFound();

            var branches = _unitOfWork.Branches.GetAll();
            WarehouseFormViewModel viewModel = warehouse.Adapt<WarehouseFormViewModel>();
            viewModel.Branches = branches.Select(b => new SelectListItem
            {
                Text = b.Name,
                Value = b.Id.ToString()
            });
            return PartialView("_Form", viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(WarehouseFormViewModel model)
        {
            if (!model.Id.HasValue || model.Id.Value <= 0)
                return NotFound();

            if (!string.IsNullOrWhiteSpace(model.Name))
            {
                model.Name = model.Name.Trim();
            }

            bool isNameExists = await _unitOfWork.Warehouses
                .AsQueryable()
                .AnyAsync(w => w.Id != model.Id.Value
                            && w.Name.Trim().ToLower() == model.Name.ToLower()
                            && w.LastAction != LastActionName.Delete);

            if (isNameExists)
            {
                ModelState.AddModelError(nameof(model.Name), "اسم المخزن موجود بالفعل.");
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
            var warehouse = await _unitOfWork.Warehouses.GetById(model.Id.Value);
            if (warehouse == null)
            {
                return NotFound();
            }
            warehouse.Name = model.Name;
            warehouse.BranchId = model.SelectedBranch; 
            warehouse.LastAction = LastActionName.Update;
            warehouse.UpdatedById = User.GetUserId();
            warehouse.UpdatedOn = DateTime.Now;
            _unitOfWork.Warehouses.Update(warehouse);
            await _unitOfWork.SaveChangesAsync();

            var viewModel = warehouse.Adapt<WarehouseViewModel>();
            viewModel.LastAction = warehouse.LastAction;
            var selectedBranch = await _unitOfWork.Branches.GetById(model.SelectedBranch);
            if (selectedBranch != null)
            {
                viewModel.BranchName = selectedBranch.Name;
            }
            return PartialView("_Row", viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var warehouse = await _unitOfWork.Warehouses.GetById(id);
            if (warehouse is null)
                return NotFound();
            bool hasProducts = _unitOfWork.ProductWarehouses
                .GetAll(x => x.WarehouseId == id && x.LastAction != LastActionName.Delete)
                .Any();

            if (hasProducts)
            {
                Response.StatusCode = 400;
                return Content("لا يمكن حذف هذا المخزن  لأنه مرتبط بمنتجات نشطة.");
            }

            warehouse.LastAction = LastActionName.Delete;
            warehouse.UpdatedById = User.GetUserId();
            warehouse.UpdatedOn = DateTime.Now;
            _unitOfWork.Warehouses.Update(warehouse);
            await _unitOfWork.SaveChangesAsync();

            var viewModel = warehouse.Adapt<WarehouseViewModel>();
            viewModel.LastAction = warehouse.LastAction;

            var selectedBranch = await _unitOfWork.Branches.GetById(warehouse.BranchId);
            if (selectedBranch != null)
            {
                viewModel.BranchName = selectedBranch.Name;
            }
            return PartialView("_Row", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id)
        {
            var warehouse = await _unitOfWork.Warehouses.GetById(id);
            if (warehouse == null)
            {
                return NotFound();
            }
            warehouse.LastAction = LastActionName.Update;
            warehouse.UpdatedById = User.GetUserId();
            warehouse.UpdatedOn = DateTime.Now;
            _unitOfWork.Warehouses.Update(warehouse);
            await _unitOfWork.SaveChangesAsync();

            var viewModel = warehouse.Adapt<WarehouseViewModel>();
            viewModel.LastAction = warehouse.LastAction;

            var selectedBranch = await _unitOfWork.Branches.GetById(warehouse.BranchId);
            if (selectedBranch != null)
            {
                viewModel.BranchName = selectedBranch.Name;
            }
            return PartialView("_Row", viewModel);
        }
    }
}