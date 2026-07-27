using Microsoft.AspNetCore.Mvc.Rendering;

namespace WarehouseBLL.BusinessServices.View_Models.Reports
{
    public class BranchInventoryViewModel
    {
        public int? SelectedBranchId { get; set; }
        public IEnumerable<SelectListItem> Branches { get; set; } = [];
        public List<string> WarehouseNames { get; set; } = [];
        public List<BranchInventoryItemViewModel> Items { get; set; } = [];
        public decimal GrandTotal { get; set; }
    }

    public class BranchInventoryItemViewModel
    {
        public string ProductName { get; set; } = string.Empty;
        public Dictionary<string, decimal> QuantityByWarehouse { get; set; } = [];
        public decimal TotalQuantity { get; set; }
    }
}
