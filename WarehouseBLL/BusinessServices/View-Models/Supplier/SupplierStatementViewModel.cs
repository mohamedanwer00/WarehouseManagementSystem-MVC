using Microsoft.AspNetCore.Mvc.Rendering;
using WarehouseDAL.Entities.Enums;

namespace WarehouseBLL.BusinessServices.View_Models.Supplier
{
    public class SupplierStatementViewModel
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public IEnumerable<SelectListItem> Suppliers { get; set; } = [];
        public decimal OpeningBalance { get; set; }
        public BalanceType OpeningBalanceType { get; set; }
        public List<StatementLineViewModel> Lines { get; set; } = [];
        public decimal TotalDebtor { get; set; }
        public decimal TotalCreditor { get; set; }
        public decimal ClosingBalance { get; set; }
        public bool HasTransactions { get; set; }
    }

    public class StatementLineViewModel
    {
        public DateTime Date { get; set; }
        public string? Notes { get; set; }
        public decimal? DebtorAmount { get; set; }
        public decimal? CreditAmount { get; set; }
        public decimal RunningBalance { get; set; }
        public int? InvoiceId { get; set; }
        public string? InvoiceNumber { get; set; }
        public bool IsPurchaseInvoice { get; set; }
    }
}
