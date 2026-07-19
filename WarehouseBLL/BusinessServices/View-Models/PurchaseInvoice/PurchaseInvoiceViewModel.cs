using System;
using System.Collections.Generic;
using System.Text;
using WarehouseDAL.Entities.Enums;

namespace WarehouseBLL.BusinessServices.View_Models.PurchaseInvoice
{
    public class PurchaseInvoiceViewModel
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = null!;
        public DateTime InvoiceDate { get; set; }
        public string SupplierName { get; set; } = null!; 
        public string BranchName { get; set; } = null!;   
        public string WarehouseName { get; set; } = null!;
        public string? CashBoxName { get; set; }          

        public decimal Total { get; set; }
        public decimal Discount { get; set; }
        public decimal Net { get; set; }
        public decimal Paid { get; set; }
        public decimal Remaining { get; set; }

        public PaymentMethod PaymentMethod { get; set; }
        public string LastAction { get; set; } = null!;
    }
}
