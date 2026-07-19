using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using WarehouseBLL.FormViewModels.PurchaseInvoiceItem;
using WarehouseDAL.Entities.Enums;

namespace WarehouseBLL.FormViewModels.PurchaseInvoice
{
    public class PurchaseInvoiceFormViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "رقم الفاتورة مطلوب")]
        public string InvoiceNumber { get; set; } = null!;

        [Required(ErrorMessage = "تاريخ الفاتورة مطلوب")]
        public DateTime InvoiceDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "يجب اختيار المورد")]
        public int SupplierId { get; set; }

        [Required(ErrorMessage = "يجب اختيار الفرع")]
        public int BranchId { get; set; }

        [Required(ErrorMessage = "يجب اختيار المخزن")]
        public int WarehouseId { get; set; }

        public int? CashBoxId { get; set; } 

        public decimal Total { get; set; } 
        public decimal Discount { get; set; } 
        public decimal Net { get; set; } 
        public decimal Paid { get; set; } 
        public decimal Remaining { get; set; }//المبلغ المتبقى  

        public string? Notes { get; set; }

        [Required(ErrorMessage = "يجب اختيار طريقة الدفع")]
        public PaymentMethod PaymentMethod { get; set; }

        public List<PurchaseInvoiceItemFormViewModel> Items { get; set; } = new List<PurchaseInvoiceItemFormViewModel>();

        public List<SelectListItem> Branches { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Warehouses { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Suppliers { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> CashBoxes { get; set; } = new List<SelectListItem>();
    }
}
