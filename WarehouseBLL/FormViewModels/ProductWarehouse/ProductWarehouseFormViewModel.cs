using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WarehouseBLL.FormViewModels.ProductWarehouse
{
    public class ProductWarehouseFormViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "يجب اختيار المنتج")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "يجب اختيار المخزن")]
        public int WarehouseId { get; set; }

        [Required(ErrorMessage = "يجب اختيار الوحدة")]
        public int UnitId { get; set; }

        [Required(ErrorMessage = "الكمية مطلوبة")]
        [Range(0, double.MaxValue, ErrorMessage = "يجب أن تكون الكمية صفر أو أكبر")]
        public decimal Quantity { get; set; }
    }
}
