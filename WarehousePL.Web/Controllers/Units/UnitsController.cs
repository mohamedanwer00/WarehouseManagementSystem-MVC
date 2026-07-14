using Microsoft.AspNetCore.Mvc;
using WarehouseBLL.BusinessServices.View_Models.Unit;
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
        public IActionResult Create(UnitFormViewModel model)
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

            _unitOfWork.Units.Add(unit);
            _unitOfWork.SaveChanges();

            var viewModel = unit.Adapt<UnitViewModel>();
            viewModel.LastAction = unit.LastAction;

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var unit = _unitOfWork.Units.GetById(id);
            if (unit == null)
                return NotFound();

            var viewModel = unit.Adapt<UnitFormViewModel>();
            return PartialView("_Form", viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(UnitFormViewModel model)
        {
            var unit = _unitOfWork.Units.GetById(model.Id.Value);
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

            _unitOfWork.Units.Update(unit);
            _unitOfWork.SaveChanges();

            var viewModel = unit.Adapt<UnitViewModel>();
            viewModel.LastAction = unit.LastAction;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var unit = _unitOfWork.Units.GetById(id);
            if (unit is null)
                return NotFound();

            unit.LastAction = LastActionName.Delete;
            _unitOfWork.Units.Update(unit);
            _unitOfWork.SaveChanges();

            var viewModel = unit.Adapt<UnitViewModel>();
            viewModel.LastAction = unit.LastAction;

            return PartialView("_Row", viewModel);
        }

        [HttpPost]
        public IActionResult Restore(int id)
        {
            var unit = _unitOfWork.Units.GetById(id);
            if (unit == null) return NotFound();

            unit.LastAction = LastActionName.Update;
            _unitOfWork.Units.Update(unit);
            _unitOfWork.SaveChanges();

            var viewModel = unit.Adapt<UnitViewModel>();
            viewModel.LastAction = unit.LastAction;

            return PartialView("_Row", viewModel);
        }
    }
}