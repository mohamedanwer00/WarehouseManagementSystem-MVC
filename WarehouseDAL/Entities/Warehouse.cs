using System;
using System.Collections.Generic;
using System.Text;
using WarehouseDAL.Entities.Entities;

namespace WarehouseDAL.Entities
{
    public class Warehouse: BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public int BranchId { get; set; }
        public Branch Branch { get; set; } = null!;
    }
}
