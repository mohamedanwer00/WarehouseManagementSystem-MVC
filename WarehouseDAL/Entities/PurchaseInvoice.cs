using WarehouseDAL.Entities.Entities;
using WarehouseDAL.Entities.Enums;

namespace WarehouseDAL.Entities
{
    public class PurchaseInvoice : BaseEntity
    {
        public string InvoiceNumber { get; set; } = null!;
        public DateTime InvoiceDate { get; set; } = DateTime.Now;
        public decimal? Discount { get; set; }
        public decimal TotalAmount { get; set; } // PurchaseInvoiceItems.TotalPrice - Discount
        public decimal? Paid { get; set; }//المدفوع
        public decimal? Remaining { get; set; }//المتبقي
        public string? Notes { get; set; }


        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; } = null!;
        public int BranchId { get; set; }
        public Branch Branch { get; set; } = null!;
        public int WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; } = null!;
        public InvoiceStatus Status { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public ICollection<PurchaseInvoiceItem> PurchaseInvoiceItems { get; set; } = [];
    }

    public class PurchaseInvoiceItem
    {
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int ProductUnitId { get; set; }
        public ProductUnit ProductUnit { get; set; } = null!;

        public decimal PurchasePrice { get; set; }

        public double Quantity { get; set; }

        public decimal TotalPrice { get; set; } // (quantity * PurchasePrice - Discount)

        public decimal? Discount { get; set; }
    }
}

