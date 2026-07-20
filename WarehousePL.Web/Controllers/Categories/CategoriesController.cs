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
        public async Task<IActionResult> Create(CategoryFormViewModel model)
        {
            // تحقق من تكرار الاسم
            var isDuplicate = _unitOfWork.Categories
                .GetAll(c => c.Name.Trim().ToLower() == model.Name.Trim().ToLower()
                          && c.LastAction != LastActionName.Delete)
                .Any();

            if (isDuplicate)
                ModelState.AddModelError(nameof(model.Name), "هذا الاسم مستخدم بالفعل.");

            if (!ModelState.IsValid)
                return PartialView("_Form", model);

            var category = model.Adapt<Category>();
            category.LastAction = LastActionName.Insert;
            category.CreatedById = User.GetUserId();
            category.CreatedOn = DateTime.Now;

             await _unitOfWork.Categories.AddAsync(category);
            _unitOfWork.SaveChanges();

            var viewModel = category.Adapt<CategoryViewModel>();
            viewModel.LastAction = category.LastAction;

            return PartialView("_Row", viewModel);
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
        public async Task<IActionResult> Edit(CategoryFormViewModel model)
        {
            // تحقق من تكرار الاسم (ما عدا نفس العنصر)
            var isDuplicate = _unitOfWork.Categories
                .GetAll(c => c.Name.Trim().ToLower() == model.Name.Trim().ToLower()
                          && c.Id != model.Id
                          && c.LastAction != LastActionName.Delete)
                .Any();

            if (isDuplicate)
                ModelState.AddModelError(nameof(model.Name), "هذا الاسم مستخدم بالفعل.");

            if (!ModelState.IsValid)
                return PartialView("_Form", model);

            var category = await _unitOfWork.Categories.GetById(model.Id!.Value);

            if (category is null)
                return NotFound();

            model.Adapt(category);
            category.LastAction = LastActionName.Update;
            category.UpdatedById = User.GetUserId();
            category.UpdatedOn = DateTime.Now;

            _unitOfWork.Categories.Update(category);
            _unitOfWork.SaveChanges();

            var viewModel = category.Adapt<CategoryViewModel>();
            viewModel.LastAction = category.LastAction;

            return PartialView("_Row", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _unitOfWork.Categories.GetById(id);

            if (category is null)
                return NotFound();

            // منع الحذف لو الكاتيجوري مرتبطة بمنتج نشط
            var hasProducts = _unitOfWork.Products
                .GetAll(p => p.CategoryId == id && p.LastAction != LastActionName.Delete)
                .Any();

            if (hasProducts)
            {
                Response.StatusCode = 400;
                return Content("لا يمكن حذف هذه الفئة لأنها مرتبطة بمنتجات نشطة.");
            }

            category.LastAction = LastActionName.Delete;
            category.UpdatedById = User.GetUserId();
            category.UpdatedOn = DateTime.Now;

            _unitOfWork.Categories.Update(category);
            _unitOfWork.SaveChanges();

            var viewModel = category.Adapt<CategoryViewModel>();
            viewModel.LastAction = category.LastAction;

            return PartialView("_Row", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id)
        {
            var category = await _unitOfWork.Categories.GetById(id);

            if (category is null)
                return NotFound();

            category.LastAction = LastActionName.Update;
            category.UpdatedById = User.GetUserId();
            category.UpdatedOn = DateTime.Now;

            _unitOfWork.Categories.Update(category);
            _unitOfWork.SaveChanges();

            var viewModel = category.Adapt<CategoryViewModel>();
            viewModel.LastAction = category.LastAction;

            return PartialView("_Row", viewModel);
        }
    }
}
