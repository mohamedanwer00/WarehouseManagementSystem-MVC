using Microsoft.AspNetCore.Mvc;
using WarehouseBLL.BusinessServices.View_Models.ProductWarehouse;

namespace WarehousePL.Web.Controllers.ProductWarehouses
{
    public class ProductWarehousesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductWarehousesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var stockList = _unitOfWork.ProductWarehouse.GetTableNoTracking()
                .Include(x => x.Product)
                    .ThenInclude(p => p.ProductUnits)
                        .ThenInclude(pu => pu.Unit)
                .Include(x => x.Warehouse)
                .ToList();

            // بناء الـ ViewModel يدوياً عشان نجيب اسم الوحدة الأساسية
            var viewModel = stockList.Select(x =>
            {
                var baseUnit = x.Product?.ProductUnits?.FirstOrDefault(pu => pu.IsBaseUnit);
                return new ProductWarehouseViewModel
                {
                    Id            = x.Id,
                    ProductId     = x.ProductId,
                    ProductName   = x.Product?.Name ?? "",
                    ProductCode   = x.Product?.Code ?? "",
                    WarehouseId   = x.WarehouseId,
                    WarehouseName = x.Warehouse?.Name ?? "",
                    UnitId        = baseUnit?.UnitId ?? 0,
                    UnitName      = baseUnit?.Unit?.Name ?? "—",
                    Quantity      = x.Quantity
                };
            }).ToList();

            return View(viewModel);
        }
    }
}
