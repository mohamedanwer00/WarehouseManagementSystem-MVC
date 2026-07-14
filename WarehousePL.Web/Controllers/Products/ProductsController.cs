using Microsoft.AspNetCore.Mvc;
using WarehouseBLL.BusinessServices.View_Models.Product;
using WarehouseBLL.FormViewModels.Product;

namespace WarehousePL.Web.Controllers.Products
{
    public class ProductsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<ProductsController> _localization;

        public ProductsController(IUnitOfWork unitOfWork,
            IStringLocalizer<ProductsController> localization)
        {
            _unitOfWork = unitOfWork;
            _localization = localization;
        }

        public IActionResult Index()
        {
            var products = _unitOfWork.Products.GetTableNoTracking()
                    .Include(p => p.Category)
                    .Include(p => p.ProductUnits)
                        .ThenInclude(pu => pu.Unit);

            var viewModel = products.Adapt<IEnumerable<ProductViewModel>>();
            return View(viewModel);
        }
        [HttpGet]
        public IActionResult Create()
        {
            var model = new ProductFormViewModel();
            PopulateDropdowns(model);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductFormViewModel model)
        {
            var isCodeExists = _unitOfWork.Products.GetAll().Any(p => p.Code.Trim() == model.Code.Trim() && !p.IsDeleted);
            if (isCodeExists)
                ModelState.AddModelError(nameof(model.Code), "كود المنتج مسجل بالفعل لمنتج آخر.");

            if (model.ProductUnits.Count(pu => pu.IsBaseUnit) != 1)
                ModelState.AddModelError("", "يجب اختيار وحدة أساسية واحدة فقط للمنتج.");

            if (!ModelState.IsValid)
            {
                PopulateDropdowns(model);
                return View(model);
            }

            var product = model.Adapt<Product>();
            product.LastAction = LastActionName.Insert;

            _unitOfWork.Products.Add(product);
            _unitOfWork.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            //var product = _unitOfWork.Products.GetTableNoTracking()
            //   .Include(p => p.ProductUnits)
            //   .FirstOrDefault(p => p.Id == id);

            //if (product == null)
            //    return NotFound();

            //var model = product.Adapt<ProductFormViewModel>();
            //PopulateDropdowns(model);

            //return View(model);

            var product = _unitOfWork.Products.GetTableNoTracking()
                .Include(p => p.ProductUnits)
                .FirstOrDefault(p => p.Id == id);

            if (product == null) return NotFound();

            var model = product.Adapt<ProductFormViewModel>();

            model.ProductUnits = product.ProductUnits.Select(pu => new ProductUnitFormViewModel
            {
                UnitId = pu.UnitId,
                Factor = pu.Factor,
                IsBaseUnit = pu.IsBaseUnit,
                PurchasePrice = pu.PurchasePrice,
                SellingPrice = pu.SellingPrice
            }).ToList();

            PopulateDropdowns(model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProductFormViewModel model)
        {
            var product = _unitOfWork.Products.GetById(model.Id.Value);
            if (product == null) return NotFound();

            if (model.ProductUnits.Count(pu => pu.IsBaseUnit) != 1)
                ModelState.AddModelError("", "يجب اختيار وحدة أساسية واحدة فقط للمنتج.");

            if (!ModelState.IsValid)
            {
                PopulateDropdowns(model);
                return View(model);
            }

            product.Name = model.Name;
            product.Code = model.Code;
            product.MinimumQuantity = model.MinimumQuantity;
            product.MaximumQuantity = model.MaximumQuantity;
            product.CategoryId = model.CategoryId;
            product.LastAction = LastActionName.Update;

            var oldUnits = _unitOfWork.ProductUnits.GetAll().Where(pu => pu.ProductId == product.Id).ToList();
            foreach (var ou in oldUnits)
            {
                _unitOfWork.ProductUnits.Delete(ou);
            }

            product.ProductUnits = model.ProductUnits.Adapt<ICollection<ProductUnit>>();

            _unitOfWork.Products.Update(product);
            _unitOfWork.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        private void PopulateDropdowns(ProductFormViewModel model)
        {
            model.Categories = _unitOfWork.Categories.GetAll()
                .Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() });

            model.AllAvailableUnits = _unitOfWork.Units.GetAll()
                .Select(u => new SelectListItem { Text = u.Name, Value = u.Id.ToString() });
        }
    }
}
