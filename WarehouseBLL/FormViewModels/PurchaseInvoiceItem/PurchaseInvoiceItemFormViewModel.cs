using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WarehouseBLL.FormViewModels.PurchaseInvoiceItem
{
    public class PurchaseInvoiceItemFormViewModel
    {
        [Required(ErrorMessage = "يجب اختيار المنتج")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "يجب اختيار الوحدة")]
        public int UnitId { get; set; }

        [Required(ErrorMessage = "الكمية مطلوبة")]
        [Range(0.001, double.MaxValue, ErrorMessage = "يجب أن تكون الكمية أكبر من الصفر")]
        public decimal Quantity { get; set; }

        [Required(ErrorMessage = "سعر الوحدة مطلوب")]
        [Range(0, double.MaxValue, ErrorMessage = "يجب أن يكون السعر قيمة موجبة")]
        public decimal UnitPrice { get; set; }

        public decimal Discount { get; set; }

        public decimal Total { get; set; }
    }
}
