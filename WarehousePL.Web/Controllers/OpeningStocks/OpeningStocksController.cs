using Microsoft.AspNetCore.Mvc;
using WarehouseBLL.BusinessServices.View_Models.OpeningStock;
using WarehouseBLL.FormViewModels.OpeningStock;
using WarehouseDAL.Entities;
namespace WarehousePL.Web.Controllers.OpeningStock
{
    public class OpeningStocksController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public OpeningStocksController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var stocks = _unitOfWork.OpeningStocks
                .GetTableNoTracking()//ده بيجيب الداتا بس للقراءه فقط وطبعا اسرع 
                .Include(x => x.Warehouse)
                    .ThenInclude(w => w.Branch)
                .Include(x => x.Product)
                .OrderByDescending(x => x.CreatedOn)
                .ToList();

            var viewModel = stocks.Adapt<IEnumerable<OpeningStockViewModel>>();

            return View(viewModel);
        }

        // 2. شاشة الإضافة (Server-Side Initialization)
        [HttpGet]
        public IActionResult Create(int? selectedBranchId, int? selectedWarehouseId)
        {
            var model = new OpeningStockFormViewModel
            {
                Branches = GetBranchesList()
            };

            // اختيار الفرع
            if (selectedBranchId.HasValue && selectedBranchId.Value > 0)
            {
                model.SelectedBranch = selectedBranchId.Value;
            }
            else if (model.Branches.Any())
            {
                model.SelectedBranch = int.Parse(model.Branches.First().Value);
            }

            // جلب مخازن الفرع المحدد
            if (model.SelectedBranch > 0)
            {
                model.Warehouses = GetWarehousesList(model.SelectedBranch);
            }

            // اختيار المخزن
            if (selectedWarehouseId.HasValue && selectedWarehouseId.Value > 0)
            {
                model.SelectedWarehouse = selectedWarehouseId.Value;
            }
            else if (model.Warehouses.Any())
            {
                model.SelectedWarehouse = int.Parse(model.Warehouses.First().Value);
            }

            // جلب المنتجات والكميات إذا تم اختيار مخزن
            if (model.SelectedWarehouse > 0)
            {
                model.Items = GetProductsForWarehouse(model.SelectedWarehouse);
            }

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(OpeningStockFormViewModel model)
        {
            if (!ModelState.IsValid || model.SelectedWarehouse <= 0 || model.Items == null || !model.Items.Any())
            {
                model.Branches = GetBranchesList();
                model.Warehouses = model.SelectedBranch > 0 ? GetWarehousesList(model.SelectedBranch) : new List<SelectListItem>();

                if (model.SelectedWarehouse > 0 && (model.Items == null || !model.Items.Any()))
                {
                    model.Items = GetProductsForWarehouse(model.SelectedWarehouse);
                }

                return View(model);
            }

            var existingStocks = _unitOfWork.OpeningStocks
                .GetAll(x => x.WarehouseId == model.SelectedWarehouse && x.LastAction != LastActionName.Delete)
                .ToDictionary(x => x.ProductId);// هنا بحولها الى ديكشينارى 

            var currentUserId = User.GetUserId() ?? 0;

            // الأصناف المراد تعديلها
            var toUpdate = model.Items
                .Where(item => existingStocks.ContainsKey(item.ProductId))
                .Select(item => UpdateStock(existingStocks[item.ProductId], item.Quantity))
                .ToList();

            // الأصناف الجديدة المراد إضافتها
            var toInsert = model.Items
                .Where(item => !existingStocks.ContainsKey(item.ProductId))
                .Select(item => BuildNewStock(model.SelectedWarehouse, item, currentUserId))
                .ToList();

            if (toUpdate.Any())
                _unitOfWork.OpeningStocks.UpdateRange(toUpdate);

            if (toInsert.Any())
                _unitOfWork.OpeningStocks.AddRange(toInsert);

            _unitOfWork.SaveChanges();

            TempData["SuccessMessage"] = "تم حفظ المخزون الافتتاحي بنجاح.";

            return RedirectToAction(nameof(Create), new { selectedBranchId = model.SelectedBranch, selectedWarehouseId = model.SelectedWarehouse });
        }
        // 4. Endpoints للتعامل عبر JS/AJAX إن أردت
        [HttpGet]
        public IActionResult GetWarehouses(int branchId)
        {
            return Json(GetWarehousesList(branchId));
        }

        [HttpGet]
        public IActionResult GetProducts(int warehouseId)
        {
            var model = GetProductsForWarehouse(warehouseId);
            return PartialView("_ProductsTable", model);
        }


        private List<OpeningStockItemFormViewModel> GetProductsForWarehouse(int warehouseId)
        {
            // 1. جلب الأرصدة الافتتاحية الحالية للمخزن ده وتحويلها لـ Dictionary للوصول السريع
            var existingQuantities = _unitOfWork.OpeningStocks
                .GetTableNoTracking()
                .Where(s => s.WarehouseId == warehouseId && s.LastAction != LastActionName.Delete)
                .ToDictionary(s => s.ProductId, s => s.Quantity);

            // 2. جلب المنتجات وربط الكمية فوراً من الـ Dictionary
            return _unitOfWork.Products
                .GetTableNoTracking()
                .Where(p => p.LastAction != LastActionName.Delete)
                .OrderBy(p => p.Name)
                .Select(p => new OpeningStockItemFormViewModel
                {
                    ProductId = p.Id,
                    ProductName = p.Name
                })
                .ToList()
                .Select(p => {
                    p.Quantity = existingQuantities.TryGetValue(p.ProductId, out decimal qty) ? qty : 0;
                    return p;
                })
                .ToList();
        }

        private static WarehouseDAL.Entities.OpeningStock UpdateStock(WarehouseDAL.Entities.OpeningStock stock, decimal quantity)
        {
            stock.Quantity = quantity;
            stock.LastAction = LastActionName.Update;
            return stock;
        }

        private static WarehouseDAL.Entities.OpeningStock BuildNewStock(int warehouseId, OpeningStockItemFormViewModel item, int userId)
        {
            return new WarehouseDAL.Entities.OpeningStock
            {
                WarehouseId = warehouseId,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                CreatedOn = DateTime.Now,
                CreatedById = userId,
                LastAction = LastActionName.Insert
            };
        }

        private List<SelectListItem> GetBranchesList()
        {
            return _unitOfWork.Branches
                .GetTableNoTracking()
                .Where(x => x.LastAction != LastActionName.Delete)
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                })
                .ToList();
        }

        private List<SelectListItem> GetWarehousesList(int branchId)
        {
            return _unitOfWork.Warehouses
                .GetTableNoTracking()
                .Where(x => x.LastAction != LastActionName.Delete && x.BranchId == branchId)
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                })
                .ToList();
        }

    }
}
