using System;
using System.Collections.Generic;
using System.Text;

namespace WarehouseBLL.BusinessServices.View_Models.Warehouse
{
    public class WarehouseViewModel
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? LastAction { get; set; }
        public string? BranchName { get; set; } 
    }
}
