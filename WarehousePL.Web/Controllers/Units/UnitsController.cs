using Microsoft.AspNetCore.Mvc;
using WarehouseBLL.BusinessServices.View_Models.Unit;
using WarehouseBLL.Extensions;
using WarehouseBLL.FormViewModels.Unit;

namespace WarehousePL.Web.Controllers.Units
{
    public class UnitsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<UnitsController> _stringLocalizer;

        public UnitsController(IUnitOfWork unitOfWork,
            IStringLocalizer<UnitsController> stringLocalizer)
        {
            _unitOfWork = unitOfWork;
            _stringLocalizer = stringLocalizer;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var units = _unitOfWork.Units.GetAll();
            var viewModel = units.Adapt<IEnumerable<UnitViewModel>>();
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return PartialView("_Form", new UnitFormViewModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UnitFormViewModel model)
        {
            var isNameExists = _unitOfWork.Units
                .GetAll()
                .Any(u => u.Name.Trim().ToLower() == model.Name.Trim().ToLower() && u.LastAction != LastActionName.Delete);

            if (isNameExists)
            {
                ModelState.AddModelError(nameof(model.Name), "NameAlreadyExists");
            }

            if (!ModelState.IsValid)
            {
                return PartialView("_Form", model);
            }


            var unit = model.Adapt<Unit>();
            unit.LastAction = LastActionName.Insert;
            unit.CreatedById = User.GetUserId();
            unit.CreatedOn = DateTime.Now;
            await _unitOfWork.Units.AddAsync(unit);
            await _unitOfWork.SaveChangesAsync();

            var viewModel = unit.Adapt<UnitViewModel>();
            viewModel.LastAction = unit.LastAction;

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Unit? unit = await _unitOfWork.Units.GetById(id);
            if (unit is null)
                return NotFound();

            UnitFormViewModel viewModel = unit.Adapt<UnitFormViewModel>();
            return PartialView("_Form", viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UnitFormViewModel model)
        {
            var unit = await _unitOfWork.Units.GetById(model.Id!.Value);
            if (unit == null) return NotFound();

            var isNameExists = _unitOfWork.Units
                .GetAll()
                .Any(u => u.Id != model.Id && u.Name.Trim().ToLower() == model.Name.Trim().ToLower() && u.LastAction != LastActionName.Delete);

            if (isNameExists)
            {
                ModelState.AddModelError(nameof(model.Name), "NameAlreadyExists");
            }

            if (!ModelState.IsValid)
            {
                return PartialView("_Form", model);
            }

            unit.Name = model.Name;
            unit.Symbol = model.Symbol;
            unit.LastAction = LastActionName.Update;
            unit.UpdatedById = User.GetUserId();
            unit.UpdatedOn = DateTime.Now;
            _unitOfWork.Units.Update(unit);
            await _unitOfWork.SaveChangesAsync();

            var viewModel = unit.Adapt<UnitViewModel>();
            viewModel.LastAction = unit.LastAction;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var unit = await _unitOfWork.Units.GetById(id);
            if (unit is null)
                return NotFound();

            bool isBaseUnit = _unitOfWork.ProductUnits
                .GetAll(x => x.UnitId == id
                          && x.IsBaseUnit
                          && x.LastAction != LastActionName.Delete)
                .Any();

            if (isBaseUnit)
            {
                Response.StatusCode = 400;
                return Content("لا يمكن حذف هذه الوحدة لأنها الوحدة الأساسية لأحد المنتجات.");
            }

            bool isUsed = _unitOfWork.ProductUnits
                .GetAll(x => x.UnitId == id
                          && x.LastAction != LastActionName.Delete)
                .Any();

            if (isUsed)
            {
                Response.StatusCode = 400;
                return Content("لا يمكن حذف هذه الوحدة لأنها مرتبطة بأحد المنتجات.");
            }

            unit.LastAction = LastActionName.Delete;
            unit.UpdatedById = User.GetUserId();
            unit.UpdatedOn = DateTime.Now;
            _unitOfWork.Units.Update(unit);
            await _unitOfWork.SaveChangesAsync();

            var viewModel = unit.Adapt<UnitViewModel>();
            viewModel.LastAction = unit.LastAction;

            return PartialView("_Row", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Restore(int id)
        {
            var unit = await _unitOfWork.Units.GetById(id);
            if (unit == null) return NotFound();

            unit.LastAction = LastActionName.Update;
            unit.UpdatedById = User.GetUserId();
            unit.UpdatedOn = DateTime.Now;
            _unitOfWork.Units.Update(unit);
            await _unitOfWork.SaveChangesAsync();

            var viewModel = unit.Adapt<UnitViewModel>();
            viewModel.LastAction = unit.LastAction;

            return PartialView("_Row", viewModel);
        }
    }
}