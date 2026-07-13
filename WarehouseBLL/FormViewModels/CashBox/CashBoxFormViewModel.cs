using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WarehouseBLL.FormViewModels.CashBox
{
    public class CashBoxFormViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "اسم الخزنة مطلوب")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "يجب تحديد الفرع")]
        public int SelectedBranch { get; set; }

        [Required(ErrorMessage = "الرصيد الافتتاحي مطلوب")]
        [Range(0, double.MaxValue, ErrorMessage = "الرصيد الافتتاحي لا يمكن أن يكون بالسالب")]
        public decimal OpeningBalance { get; set; }

        public IEnumerable<SelectListItem>? Branches { get; set; }
    }
}
