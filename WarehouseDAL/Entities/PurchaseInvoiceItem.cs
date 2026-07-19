using System;
using System.Collections.Generic;
using System.Text;

namespace WarehouseDAL.Entities
{
    public class PurchaseInvoiceItem:BaseEntity
    {
        public int PurchaseInvoiceId { get; set; }
        public PurchaseInvoice PurchaseInvoice { get; set; } = null!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int UnitId { get; set; }
        public Unit? Unit { get; set; }

        public decimal Quantity { get; set; } // الكمية المشتراة
        public decimal UnitPrice { get; set; } // سعر شراء الوحدة الواحدة
        public decimal Discount { get; set; } // الخصم على مستوى الصنف نفسه (إن وجد)
        public decimal Total { get; set; } // إجمالي السطر = (الكمية × السعر) - الخصم
    }
}
