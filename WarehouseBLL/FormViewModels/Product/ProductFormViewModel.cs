using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WarehouseBLL.FormViewModels.Product
{
    public class ProductFormViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "اسم المنتج مطلوب")]
        [StringLength(100, ErrorMessage = "الاسم طويل جداً")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "كود المنتج مطلوب")]
        [StringLength(50, ErrorMessage = "الكود طويل جداً")]
        public string Code { get; set; } = null!;

        [Required(ErrorMessage = "الحد الأدنى مطلوب")]
        [Range(0, double.MaxValue, ErrorMessage = "يجب أن تكون القيمة موجبة")]
        public decimal MinimumQuantity { get; set; }

        [Required(ErrorMessage = "الحد الأقصى مطلوب")]
        [Range(0, double.MaxValue, ErrorMessage = "يجب أن تكون القيمة موجبة")]
        public decimal MaximumQuantity { get; set; }

        [Required(ErrorMessage = "يجب اختيار القسم")]
        public int CategoryId { get; set; }

        public IEnumerable<SelectListItem>? Categories { get; set; }
        public IEnumerable<SelectListItem>? AllAvailableUnits { get; set; } 

        public List<ProductUnitFormViewModel> ProductUnits { get; set; } = [];
    }
}
