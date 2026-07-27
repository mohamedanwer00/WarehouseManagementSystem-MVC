using Microsoft.AspNetCore.Mvc.Rendering;

namespace WarehouseBLL.BusinessServices.View_Models.Reports
{
    public class ItemMovementViewModel
    {
        public int? SelectedProductId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public IEnumerable<SelectListItem> Products { get; set; } = [];
        public List<ItemMovementLineViewModel> Lines { get; set; } = [];
        public decimal TotalIn { get; set; }
        public decimal TotalOut { get; set; }
        public decimal NetBalance { get; set; }
        public bool HasMovements { get; set; }
    }

    public class ItemMovementLineViewModel
    {
        public DateTime Date { get; set; }
        public string TransactionTypeName { get; set; } = string.Empty;
        public decimal? InQuantity { get; set; }
        public decimal? OutQuantity { get; set; }
        public decimal RunningBalance { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string WarehouseName { get; set; } = string.Empty;
        public string? ReferenceNumber { get; set; }
    }
}
