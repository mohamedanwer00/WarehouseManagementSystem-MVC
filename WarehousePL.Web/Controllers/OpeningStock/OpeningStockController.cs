using Microsoft.AspNetCore.Mvc;
using WarehouseBLL.BusinessServices.View_Models.OpeningStock;
using WarehouseBLL.FormViewModels.OpeningStock;

namespace WarehousePL.Web.Controllers.OpeningStock
{
    public class OpeningStockController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public OpeningStockController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task <IActionResult> Index()
        {
            var stocks = await _unitOfWork.OpeningStocks.AsQueryable()
                .Include(x => x.Warehouse)
                .ThenInclude(x => x.Branch)
                .Include(x => x.Product)
                .ToListAsync();

            var viewModel = stocks.Adapt<IEnumerable<OpeningStockViewModel>>();

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            OpeningStockFormViewModel model = new OpeningStockFormViewModel
            {
                Branches = GetBranchesList(),
                Warehouses = new List<SelectListItem>(),
                Items = new List<OpeningStockItemFormViewModel>()
            };

            return View(model);
        }

        private List<SelectListItem> GetBranchesList()
        {
            return _unitOfWork.Branches.GetTableNoTracking()
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name })
                .ToList();
        }

        private List<SelectListItem> GetWarehousesByBranchList(int branchId)
        {
            return _unitOfWork.Warehouses.GetTableNoTracking()
                .Where(x => x.BranchId == branchId)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name })
                .ToList();
        }

        private List<SelectListItem> GetProductsList()
        {
            return _unitOfWork.Products.GetTableNoTracking()
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = $"{x.Code} - {x.Name}" })
                .ToList();
        }

    }
}
