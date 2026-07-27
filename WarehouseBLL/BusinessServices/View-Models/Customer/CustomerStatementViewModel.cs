using Microsoft.AspNetCore.Mvc.Rendering;
using WarehouseDAL.Entities.Enums;
using WarehouseBLL.BusinessServices.View_Models.Supplier;

namespace WarehouseBLL.BusinessServices.View_Models.Customer
{
    public class CustomerStatementViewModel
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public IEnumerable<SelectListItem> Customers { get; set; } = [];
        public decimal OpeningBalance { get; set; }
        public BalanceType OpeningBalanceType { get; set; }
        public List<StatementLineViewModel> Lines { get; set; } = [];
        public decimal TotalDebtor { get; set; }
        public decimal TotalCreditor { get; set; }
        public decimal ClosingBalance { get; set; }
        public bool HasTransactions { get; set; }
    }
}
