using Mapster;
using Microsoft.AspNetCore.Mvc;
using WarehouseBLL.BusinessServices.View_Models;
using WarehouseBLL.Const;
using WarehouseBLL.FormViewModels.Category;
using WarehouseDAL.Entities;
using WarehouseDAL.Repositories.Interfaces;

namespace WarehousePL.Web.Controllers.Categories
{
    public class CategoriesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoriesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var categories = _unitOfWork.Categories.GetAll();

            var viewModel = categories.Adapt<IEnumerable<CategoryViewModel>>();

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return PartialView("_Form");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_Form", model);

            var category = model.Adapt<Category>();

            category.LastAction = LastActionName.Insert;

            _unitOfWork.Categories.Add(category);
            _unitOfWork.SaveChanges();

            var viewModel = category.Adapt<CategoryViewModel>();
            viewModel.LastAction = category.LastAction;

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _unitOfWork.Categories.GetById(id);

            if (category is null)
                return NotFound();

            var form = category.Adapt<CategoryFormViewModel>();

            return PartialView("_Form", form);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_Form", model);

            var category = _unitOfWork.Categories.GetById(model.Id!.Value);

            if (category is null)
                return NotFound();

            model.Adapt(category);

            category.UpdatedOn = DateTime.Now;
            category.LastAction = LastActionName.Update;

            _unitOfWork.Categories.Update(category);
            _unitOfWork.SaveChanges();

            var viewModel = category.Adapt<CategoryViewModel>();
            viewModel.LastAction = category.LastAction;

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var category = _unitOfWork.Categories.GetById(id);

            if (category is null)
                return NotFound();

            category.IsDeleted = true;
            category.LastAction = LastActionName.Delete;

            _unitOfWork.Categories.Update(category);
            _unitOfWork.SaveChanges();

            var viewModel = category.Adapt<CategoryViewModel>();
            viewModel.LastAction = category.LastAction;

            return PartialView("_Row", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Restore(int id)
        {
            var category = _unitOfWork.Categories.GetById(id);

            if (category is null)
                return NotFound();

            category.IsDeleted = false;
            category.LastAction = LastActionName.Insert;

            _unitOfWork.Categories.Update(category);
            _unitOfWork.SaveChanges();

            var viewModel = category.Adapt<CategoryViewModel>();
            viewModel.LastAction = category.LastAction;

            return PartialView("_Row", viewModel);
        }
    }
}