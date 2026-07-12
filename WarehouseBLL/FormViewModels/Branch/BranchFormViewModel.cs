using System;
using System.Collections.Generic;
using System.Text;

namespace WarehouseBLL.FormViewModels.Branch
{
    public class BranchFormViewModel
    {
        public int? Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
    }
}
