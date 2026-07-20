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
            var isNameExists = _unitOfWork.Warehouses
                .GetAll()
                .Any(w => w.Name.Trim().ToLower() == model.Name.Trim().ToLower() && w.LastAction != LastActionName.Delete);

            if (isNameExists)
            {
                ModelState.AddModelError(nameof(model.Name), "اسم المخزن موجود بالفعل.");
                return PartialView("_Form", model);
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
            warehouse.Address = "Default Address";
            warehouse.LastAction = LastActionName.Insert;
            warehouse.CreatedById = User.GetUserId();
            warehouse.CreatedOn = DateTime.Now;

            await _unitOfWork.Warehouses.AddAsync(warehouse);
            _unitOfWork.SaveChanges();

            var viewModel = warehouse.Adapt<WarehouseViewModel>();
            viewModel.LastAction = warehouse.LastAction;

            var selectedBranch = await _unitOfWork.Branches.GetById(model.SelectedBranch);
            if (selectedBranch != null)
            {
                viewModel.BranchName = selectedBranch.Name;
            }
            //return PartialView("_Row", viewModel);
            return RedirectToAction(nameof(Index));

        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var warehouse = _unitOfWork.Warehouses.GetById(id);
            if (warehouse == null)
            {
                return NotFound();
            }
            var branches = _unitOfWork.Branches.GetAll();
            var viewModel = warehouse.Adapt<WarehouseFormViewModel>();
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
            var isNameExists = _unitOfWork.Warehouses
                .GetAll()
                .Any(w => w.Id != model.Id && w.Name.Trim().ToLower() == model.Name.Trim().ToLower() && w.LastAction != LastActionName.Delete);
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
            _unitOfWork.SaveChanges();
            var viewModel = warehouse.Adapt<WarehouseViewModel>();
            viewModel.LastAction = warehouse.LastAction;
            var selectedBranch = await _unitOfWork.Branches.GetById(model.SelectedBranch);
            if (selectedBranch != null)
            {
                viewModel.BranchName = selectedBranch.Name; // إسناد الاسم يدوياً للـ View Model
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var warehouse = await _unitOfWork.Warehouses.GetById(id);
            if (warehouse == null)
            {
                return NotFound();
            }
            warehouse.LastAction = LastActionName.Delete;
            warehouse.UpdatedById = User.GetUserId();
            warehouse.UpdatedOn = DateTime.Now;
            _unitOfWork.Warehouses.Update(warehouse);
            _unitOfWork.SaveChanges();

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
            _unitOfWork.SaveChanges();

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
