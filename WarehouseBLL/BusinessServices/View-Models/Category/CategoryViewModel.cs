using System;
using System.Collections.Generic;
using System.Text;
using WarehouseBLL.Const;

namespace WarehouseBLL.BusinessServices.View_Models
{
    public class CategoryViewModel
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? LastAction { get; set; }
    }
}
