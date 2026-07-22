using System;
using System.Collections.Generic;
using System.Text;
using WarehouseDAL.Entities.Enums;

namespace WarehouseDAL.Entities
{
    public class Supplier : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Address { get; set; } = null!;
        public decimal OpeningBalance { get; set; }
        public BalanceType OpeningBalanceType { get; set; } = BalanceType.Creditor;

        public decimal CurrentBalance { get; set; }
        public ICollection<PurchaseInvoice> PurchaseInvoices { get; set; } = new List<PurchaseInvoice>();

    }
}
