using System;
using System.Collections.Generic;
using System.Text;

namespace WarehouseBLL.BusinessServices.View_Models.Branch
{
    public class BranchViewModel
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? LastAction { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }

    }
}
