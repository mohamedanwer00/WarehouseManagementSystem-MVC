using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WarehouseBLL.FormViewModels.Unit
{
    public class UnitFormViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "اسم الوحدة مطلوب")]
        [StringLength(50, ErrorMessage = "الاسم لا يجب أن يتخطى 50 حرف")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "رمز الوحدة مطلوب")]
        [StringLength(10, ErrorMessage = "الرمز لا يجب أن يتخطى 10 أحرف")]
        public string Symbol { get; set; } = null!;
    }
}
