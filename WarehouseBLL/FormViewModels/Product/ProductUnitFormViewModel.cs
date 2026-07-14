using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WarehouseBLL.FormViewModels.Product
{
    public class ProductUnitFormViewModel
    {

        [Required(ErrorMessage = "يجب اختيار الوحدة")]
        public int UnitId { get; set; }

        public string? UnitName { get; set; } 

        [Required(ErrorMessage = "معامل التحويل مطلوب")]
        [Range(1, int.MaxValue, ErrorMessage = "المعامل يجب أن يكون 1 على الأقل")]
        public int Factor { get; set; } = 1;

        public bool IsBaseUnit { get; set; }

        [Required(ErrorMessage = "سعر الشراء مطلوب")]
        [Range(0, double.MaxValue, ErrorMessage = "السعر لا يمكن أن يكون سالباً")]
        public decimal PurchasePrice { get; set; }

        [Required(ErrorMessage = "سعر البيع مطلوب")]
        [Range(0, double.MaxValue, ErrorMessage = "السعر لا يمكن أن يكون سالباً")]
        public decimal SellingPrice { get; set; }
    }
}
