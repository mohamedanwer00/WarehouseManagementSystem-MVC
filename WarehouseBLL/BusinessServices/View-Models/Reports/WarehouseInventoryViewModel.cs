using Microsoft.AspNetCore.Mvc.Rendering;

namespace WarehouseBLL.BusinessServices.View_Models.Reports
{
    public class WarehouseInventoryViewModel
    {
        public int? SelectedBranchId { get; set; }
        public int? SelectedWarehouseId { get; set; }
        public IEnumerable<SelectListItem> Branches { get; set; } = [];
        public IEnumerable<SelectListItem> Warehouses { get; set; } = [];
        public List<WarehouseInventoryItemViewModel> Items { get; set; } = [];
        public int TotalProducts { get; set; }
        public decimal TotalQuantity { get; set; }
    }

    public class WarehouseInventoryItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal TotalIn { get; set; }
        public decimal TotalOut { get; set; }
        public decimal Available { get; set; }
        public List<InventoryTransactionLineViewModel> Movements { get; set; } = [];
    }

    public class InventoryTransactionLineViewModel
    {
        public DateTime Date { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public string? ReferenceNumber { get; set; }
    }
}
