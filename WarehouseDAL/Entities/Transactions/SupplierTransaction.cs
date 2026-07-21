using System;
using System.Collections.Generic;
using System.Text;

namespace WarehouseDAL.Entities.Transactions
{
    public class SupplierTransaction : BaseEntity
    {
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; } = null!;
        public decimal Amount { get; set; }
        public BalanceType BalanceType { get; set; }
        public int? PurchaseInvoiceId { get; set; }
        public PurchaseInvoice? PurchaseInvoice { get; set; }
        public string? Notes { get; set; }
        public DateTime Date { get; set; }
    }
}
