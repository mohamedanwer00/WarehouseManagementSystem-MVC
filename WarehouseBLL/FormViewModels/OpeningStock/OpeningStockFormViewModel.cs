using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WarehouseBLL.FormViewModels.OpeningStock
{
    public class OpeningStockFormViewModel
    {
        public int? Id { get; set; }

        [Display(Name = "الفرع")]
        [Required(ErrorMessage = "الفرع مطلوب")]
        public int SelectedBranch { get; set; }

        public IEnumerable<SelectListItem> Branches { get; set; } = [];

        [Display(Name = "المخزن")]
        [Required(ErrorMessage = "المخزن مطلوب")]
        public int SelectedWarehouse { get; set; }

        public IEnumerable<SelectListItem> Warehouses { get; set; } = [];

        public List<OpeningStockItemFormViewModel> Items { get; set; } = [];

    }

    public class OpeningStockItemFormViewModel
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = null!;

        [Display(Name = "الكمية")]
        [Required(ErrorMessage = "الكمية مطلوبة")]
        [Range(0.01, double.MaxValue, ErrorMessage = "يجب أن تكون الكمية أكبر من صفر")]
        public decimal Quantity { get; set; }
    }
}
