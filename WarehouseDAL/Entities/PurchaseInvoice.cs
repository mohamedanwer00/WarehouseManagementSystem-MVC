using System;
using System.Collections.Generic;
using System.Text;
using WarehouseDAL.Entities.Entities;
using WarehouseDAL.Entities.Enums;

namespace WarehouseDAL.Entities
{
    public class PurchaseInvoice : BaseEntity
    {
        public string InvoiceNumber { get; set; } = null!;
        public DateTime InvoiceDate { get; set; } = DateTime.Now;
        public decimal Discount { get; set; }
        public decimal Tax { get; set; } //ضريبة القيمة المضافة
        public decimal Total { get; set; }
        public decimal Paid { get; set; }//المدفوع
        public decimal Remaining { get; set; }//المتبقي
        public string? Notes { get; set; }


        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; } = null!;
        public int BranchId { get; set; }
        public Branch Branch { get; set; } = null!;
        public int WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; } = null!;
        public InvoiceStatus Status { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }
}

