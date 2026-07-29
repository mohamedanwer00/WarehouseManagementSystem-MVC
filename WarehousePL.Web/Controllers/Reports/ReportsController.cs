using WarehouseBLL.BusinessServices.View_Models.Reports;

namespace WarehousePL.Web.Controllers.Reports;

public class ReportsController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public ReportsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> BranchInventory(int? selectedBranchId)
    {
        var model = new BranchInventoryViewModel
        {
            SelectedBranchId = selectedBranchId,
            Branches = await GetSelectListAsync(_unitOfWork.Branches.GetAll(x => x.LastAction != LastActionName.Delete))
        };

        if (!selectedBranchId.HasValue || selectedBranchId <= 0)
            return View(model);

        var warehouses = await _unitOfWork.Warehouses
            .GetAll(x => x.BranchId == selectedBranchId && x.LastAction != LastActionName.Delete)
            .OrderBy(x => x.Id)
            .ToListAsync();

        model.WarehouseNames = warehouses.Select(w => w.Name).ToList();
        var warehouseIds = warehouses.Select(w => w.Id).ToList();

        var productWarehouses = await _unitOfWork.ProductWarehouses
            .GetTableNoTracking()
            .Include(pw => pw.Product)
            .Where(pw => warehouseIds.Contains(pw.WarehouseId))
            .ToListAsync();

        var baseUnitNames = await GetBaseUnitNamesAsync(productWarehouses.Select(pw => pw.ProductId));

        model.Items = productWarehouses
            .GroupBy(pw => pw.ProductId)
            .OrderBy(g => g.First().Product.Name)
            .Select(group =>
            {
                var product = group.First().Product;
                var quantityByWarehouse = warehouses.ToDictionary(
                    w => w.Name,
                    w => group.FirstOrDefault(pw => pw.WarehouseId == w.Id)?.Quantity ?? 0);

                return new BranchInventoryItemViewModel
                {
                    ProductName = $"{product.Name} ({baseUnitNames.GetValueOrDefault(product.Id)})",
                    QuantityByWarehouse = quantityByWarehouse,
                    TotalQuantity = quantityByWarehouse.Values.Sum()
                };
            })
            .ToList();

        model.GrandTotal = model.Items.Sum(i => i.TotalQuantity);
        return View(model);
    }

    public async Task<IActionResult> WarehouseInventory(int? selectedBranchId, int? selectedWarehouseId)
    {
        var model = new WarehouseInventoryViewModel
        {
            SelectedBranchId = selectedBranchId,
            SelectedWarehouseId = selectedWarehouseId,
            Branches = await GetSelectListAsync(_unitOfWork.Branches.GetAll(x => x.LastAction != LastActionName.Delete)),
            Warehouses = selectedBranchId.HasValue && selectedBranchId > 0
                ? await GetSelectListAsync(_unitOfWork.Warehouses.GetAll(x => x.BranchId == selectedBranchId && x.LastAction != LastActionName.Delete))
                : Enumerable.Empty<SelectListItem>()
        };

        if (!selectedBranchId.HasValue || selectedBranchId <= 0 ||
            !selectedWarehouseId.HasValue || selectedWarehouseId <= 0)
            return View(model);

        var productWarehouses = await _unitOfWork.ProductWarehouses
            .GetTableNoTracking()
            .Include(pw => pw.Product)
            .Where(pw => pw.WarehouseId == selectedWarehouseId)
            .OrderBy(pw => pw.Product.Name)
            .ToListAsync();

        var productIds = productWarehouses.Select(pw => pw.ProductId).ToList();

        var transactionsByProduct = (await _unitOfWork.InventoryTransactions
                .GetTableNoTracking()
                .Where(t => t.WarehouseId == selectedWarehouseId && productIds.Contains(t.ProductId))
                .OrderByDescending(t => t.Date)
                .ToListAsync())
            .GroupBy(t => t.ProductId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var baseUnitNames = await GetBaseUnitNamesAsync(productIds);
        const int lastMovementsCount = 10;

        foreach (var pw in productWarehouses)
        {
            var productTransactions = transactionsByProduct.GetValueOrDefault(pw.ProductId, new List<InventoryTransaction>());

            model.Items.Add(new WarehouseInventoryItemViewModel
            {
                ProductId = pw.ProductId,
                ProductName = $"{pw.Product.Name} ({baseUnitNames.GetValueOrDefault(pw.ProductId)})",
                TotalIn = productTransactions
                    .Where(t => t.InventoryTransactionType is InventoryTransactionType.Purchase or InventoryTransactionType.OpeningStock)
                    .Sum(t => t.Quantity),
                TotalOut = productTransactions
                    .Where(t => t.InventoryTransactionType == InventoryTransactionType.Sell)
                    .Sum(t => t.Quantity),
                Available = pw.Quantity,
                Movements = productTransactions
                    .Take(lastMovementsCount)
                    .Select(t => new InventoryTransactionLineViewModel
                    {
                        Date = t.Date,
                        TransactionType = GetTransactionTypeName(t.InventoryTransactionType),
                        Quantity = t.Quantity,
                        ReferenceNumber = t.ReferenceNumber
                    })
                    .ToList()
            });
        }

        model.TotalProducts = model.Items.Count;
        model.TotalQuantity = model.Items.Sum(i => i.Available);
        return View(model);
    }

    public async Task<IActionResult> ItemMovement(int? selectedProductId, DateTime? dateFrom, DateTime? dateTo)
    {
        dateFrom ??= DateTime.Today.AddMonths(-1);
        dateTo ??= DateTime.Today;

        var model = new ItemMovementViewModel
        {
            SelectedProductId = selectedProductId,
            DateFrom = dateFrom,
            DateTo = dateTo,
            Products = await GetSelectListAsync(_unitOfWork.Products.GetAll(x => x.LastAction != LastActionName.Delete))
        };

        if (!selectedProductId.HasValue || selectedProductId <= 0)
            return View(model);

        DateTime searchDateFrom = dateFrom!.Value.Date;
        DateTime searchDateTo = dateTo!.Value.Date.AddDays(1).AddTicks(-1);

        var priorTransactions = await _unitOfWork.InventoryTransactions
            .GetTableNoTracking()
            .Where(t => t.ProductId == selectedProductId.Value && t.Date < searchDateFrom)
            .ToListAsync();

        decimal running = priorTransactions
            .Sum(t => t.InventoryTransactionType is InventoryTransactionType.Purchase or InventoryTransactionType.OpeningStock
                ? t.Quantity
                : -t.Quantity);

        var transactions = await _unitOfWork.InventoryTransactions
            .GetTableNoTracking()
            .Include(t => t.Branch)
            .Include(t => t.Warehouse)
            .Where(t => t.ProductId == selectedProductId.Value
                     && t.Date >= searchDateFrom
                     && t.Date <= searchDateTo)
            .OrderBy(t => t.Date)
            .ThenBy(t => t.Id)
            .ToListAsync();

        model.HasMovements = transactions.Any();

        foreach (var t in transactions)
        {
            var line = new ItemMovementLineViewModel
            {
                Date = t.Date,
                TransactionTypeName = GetTransactionTypeName(t.InventoryTransactionType),
                BranchName = t.Branch?.Name ?? "-",
                WarehouseName = t.Warehouse?.Name ?? "-",
                ReferenceNumber = t.ReferenceNumber
            };

            bool isIn = t.InventoryTransactionType is InventoryTransactionType.Purchase or InventoryTransactionType.OpeningStock;
            if (isIn)
                line.InQuantity = t.Quantity;
            else
                line.OutQuantity = t.Quantity;

            running += isIn ? t.Quantity : -t.Quantity;
            line.RunningBalance = running;
            model.Lines.Add(line);
        }

        model.TotalIn = model.Lines.Sum(l => l.InQuantity ?? 0);
        model.TotalOut = model.Lines.Sum(l => l.OutQuantity ?? 0);
        model.NetBalance = running;

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> GetWarehouses(int branchId)
    {
        var warehouses = await GetSelectListAsync(
            _unitOfWork.Warehouses.GetAll(x => x.BranchId == branchId && x.LastAction != LastActionName.Delete));

        return Json(warehouses);
    }

    // ---- Helpers مشتركة ----

    private static async Task<IEnumerable<SelectListItem>> GetSelectListAsync<T>(IQueryable<T> query)
        where T : class
    {
        // كل الكيانات هنا فيها Id + Name، فالاستعلام العام ده بيغني عن 3 دوال مكررة
        return await query
            .Select(x => new SelectListItem
            {
                Value = EF.Property<int>(x, "Id").ToString(),
                Text = EF.Property<string>(x, "Name")
            })
            .ToListAsync();
    }

    private async Task<Dictionary<int, string>> GetBaseUnitNamesAsync(IEnumerable<int> productIds)
    {
        var ids = productIds.Distinct().ToList();

        return await _unitOfWork.ProductUnits
            .GetTableNoTracking()
            .Include(pu => pu.Unit)
            .Where(pu => ids.Contains(pu.ProductId) && pu.IsBaseUnit)
            .ToDictionaryAsync(pu => pu.ProductId, pu => pu.Unit != null ? pu.Unit.Name : string.Empty);
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