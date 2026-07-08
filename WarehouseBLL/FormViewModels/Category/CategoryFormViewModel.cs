using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WarehouseBLL.FormViewModels.Category
{
    public class CategoryFormViewModel
    {
        public int? Id { get; set; }


        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = null!;
    }
}
