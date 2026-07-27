using WarehouseBLL.BusinessServices.View_Models.Reports;

namespace WarehousePL.Web.Controllers.Reports
{
    public class ReportsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult BranchInventory(int? selectedBranchId)
        {
            var model = new BranchInventoryViewModel
            {
                SelectedBranchId = selectedBranchId,
                Branches = GetBranchesList()
            };

            if (selectedBranchId.HasValue && selectedBranchId > 0)
            {
                var warehouses = _unitOfWork.Warehouses
                    .GetAll(x => x.BranchId == selectedBranchId && x.LastAction != LastActionName.Delete)
                    .OrderBy(x => x.Id)
                    .ToList();

                model.WarehouseNames = warehouses.Select(w => w.Name).ToList();
                var warehouseIds = warehouses.Select(w => w.Id).ToList();

                var productWarehouses = _unitOfWork.ProductWarehouses
                    .GetTableNoTracking()
                    .Include(pw => pw.Product)
                    .Where(pw => warehouseIds.Contains(pw.WarehouseId))
                    .ToList();

                var productGroups = productWarehouses
                    .GroupBy(pw => pw.ProductId)
                    .OrderBy(g => g.First().Product.Name)
                    .ToList();

                foreach (var group in productGroups)
                {
                    var product = group.First().Product;
                    var baseUnitName = GetBaseUnitName(product.Id);
                    var displayName = $"{product.Name} ({baseUnitName})";

                    var quantityByWarehouse = new Dictionary<string, decimal>();
                    foreach (var whName in model.WarehouseNames)
                    {
                        quantityByWarehouse[whName] = 0;
                    }

                    decimal total = 0;
                    foreach (var pw in group)
                    {
                        var wh = warehouses.FirstOrDefault(w => w.Id == pw.WarehouseId);
                        if (wh != null)
                        {
                            quantityByWarehouse[wh.Name] = pw.Quantity;
                            total += pw.Quantity;
                        }
                    }

                    model.Items.Add(new BranchInventoryItemViewModel
                    {
                        ProductName = displayName,
                        QuantityByWarehouse = quantityByWarehouse,
                        TotalQuantity = total
                    });
                }

                model.GrandTotal = model.Items.Sum(i => i.TotalQuantity);
            }

            return View(model);
        }

        public IActionResult WarehouseInventory(int? selectedBranchId, int? selectedWarehouseId)
        {
            var model = new WarehouseInventoryViewModel
            {
                SelectedBranchId = selectedBranchId,
                SelectedWarehouseId = selectedWarehouseId,
                Branches = GetBranchesList(),
                Warehouses = selectedBranchId.HasValue && selectedBranchId > 0
                    ? GetWarehousesList(selectedBranchId.Value)
                    : Enumerable.Empty<SelectListItem>()
            };

            if (selectedBranchId.HasValue && selectedBranchId > 0 &&
                selectedWarehouseId.HasValue && selectedWarehouseId > 0)
            {
                var productWarehouses = _unitOfWork.ProductWarehouses
                    .GetTableNoTracking()
                    .Include(pw => pw.Product)
                    .Where(pw => pw.WarehouseId == selectedWarehouseId)
                    .OrderBy(pw => pw.Product.Name)
                    .ToList();

                const int lastMovementsCount = 10;

                foreach (var pw in productWarehouses)
                {
                    var baseUnitName = GetBaseUnitName(pw.ProductId);
                    var displayName = $"{pw.Product.Name} ({baseUnitName})";

                    var allTransactions = _unitOfWork.InventoryTransactions
                        .GetTableNoTracking()
                        .Where(t => t.ProductId == pw.ProductId && t.WarehouseId == selectedWarehouseId)
                        .OrderByDescending(t => t.Date)
                        .ToList();

                    var movements = allTransactions
                        .Take(lastMovementsCount)
                        .OrderByDescending(t => t.Date)
                        .Select(t => new InventoryTransactionLineViewModel
                        {
                            Date = t.Date,
                            TransactionType = GetTransactionTypeName(t.InventoryTransactionType),
                            Quantity = t.Quantity,
                            ReferenceNumber = t.ReferenceNumber
                        })
                        .ToList();

                    var totalIn = allTransactions
                        .Where(t => t.InventoryTransactionType == InventoryTransactionType.Purchase
                                   || t.InventoryTransactionType == InventoryTransactionType.OpeningStock)
                        .Sum(t => t.Quantity);

                    var totalOut = allTransactions
                        .Where(t => t.InventoryTransactionType == InventoryTransactionType.Sell)
                        .Sum(t => t.Quantity);

                    model.Items.Add(new WarehouseInventoryItemViewModel
                    {
                        ProductId = pw.ProductId,
                        ProductName = displayName,
                        TotalIn = totalIn,
                        TotalOut = totalOut,
                        Available = pw.Quantity,
                        Movements = movements
                    });
                }

                model.TotalProducts = model.Items.Count;
                model.TotalQuantity = model.Items.Sum(i => i.Available);
            }

            return View(model);
        }

        public IActionResult ItemMovement(int? selectedProductId, DateTime? dateFrom, DateTime? dateTo)
        {
            var model = new ItemMovementViewModel
            {
                SelectedProductId = selectedProductId,
                DateFrom = dateFrom,
                DateTo = dateTo,
                Products = GetProductsList()
            };

            if (selectedProductId.HasValue && selectedProductId > 0 &&
                dateFrom.HasValue && dateTo.HasValue)
            {
                var transactions = _unitOfWork.InventoryTransactions
                    .GetTableNoTracking()
                    .Include(t => t.Branch)
                    .Include(t => t.Warehouse)
                    .Where(t => t.ProductId == selectedProductId
                             && t.Date >= dateFrom && t.Date <= dateTo)
                    .OrderBy(t => t.Date)
                    .ThenBy(t => t.Id)
                    .ToList();

                model.HasMovements = transactions.Any();

                decimal running = 0;
                foreach (var t in transactions)
                {
                    var line = new ItemMovementLineViewModel
                    {
                        Date = t.Date,
                        TransactionTypeName = GetTransactionTypeName(t.InventoryTransactionType),
                        BranchName = t.Branch?.Name ?? string.Empty,
                        WarehouseName = t.Warehouse?.Name ?? string.Empty,
                        ReferenceNumber = t.ReferenceNumber
                    };

                    if (t.InventoryTransactionType == InventoryTransactionType.Purchase ||
                        t.InventoryTransactionType == InventoryTransactionType.OpeningStock)
                    {
                        line.InQuantity = t.Quantity;
                        running += t.Quantity;
                    }
                    else if (t.InventoryTransactionType == InventoryTransactionType.Sell)
                    {
                        line.OutQuantity = t.Quantity;
                        running -= t.Quantity;
                    }

                    line.RunningBalance = running;
                    model.Lines.Add(line);
                }

                model.TotalIn = model.Lines.Sum(l => l.InQuantity ?? 0);
                model.TotalOut = model.Lines.Sum(l => l.OutQuantity ?? 0);
                model.NetBalance = model.TotalIn - model.TotalOut;
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult GetWarehouses(int branchId)
        {
            var warehouses = _unitOfWork.Warehouses
                .GetAll(x => x.BranchId == branchId && x.LastAction != LastActionName.Delete)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name })
                .ToList();

            return Json(warehouses);
        }

        private IEnumerable<SelectListItem> GetBranchesList()
        {
            return _unitOfWork.Branches
                .GetAll(x => x.LastAction != LastActionName.Delete)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name })
                .ToList();
        }

        private IEnumerable<SelectListItem> GetWarehousesList(int branchId)
        {
            return _unitOfWork.Warehouses
                .GetAll(x => x.BranchId == branchId && x.LastAction != LastActionName.Delete)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name })
                .ToList();
        }

        private IEnumerable<SelectListItem> GetProductsList()
        {
            return _unitOfWork.Products
                .GetAll(x => x.LastAction != LastActionName.Delete)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name })
                .ToList();
        }

        private string GetBaseUnitName(int productId)
        {
            var baseUnit = _unitOfWork.ProductUnits
                .GetTableNoTracking()
                .Include(pu => pu.Unit)
                .FirstOrDefault(pu => pu.ProductId == productId && pu.IsBaseUnit);

            return baseUnit?.Unit?.Name ?? string.Empty;
        }

        private static string GetTransactionTypeName(InventoryTransactionType type)
        {
            return type switch
            {
                InventoryTransactionType.Purchase => "شراء",
                InventoryTransactionType.Sell => "بيع",
                InventoryTransactionType.OpeningStock => "رصيد افتتاحي",
                _ => type.ToString()
            };
        }
    }
}
