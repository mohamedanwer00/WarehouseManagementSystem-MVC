using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace WarehouseBLL.FormViewModels.Warehouse
{
    public class WarehouseFormViewModel
    {
        public int? Id { get; set; }
        public string Name { get; set; } = null!;
        public int SelectedBranch { get; set; }

        public IEnumerable<SelectListItem>? Branches { get; set; }
    }
}
