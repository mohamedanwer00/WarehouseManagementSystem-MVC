using System;
using System.Collections.Generic;
using System.Text;

namespace WarehouseDAL.Entities.Entities
{
    public class Branch : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public ICollection<CashBox> CashBoxes { get; set; } = new List<CashBox>();
        public ICollection<Warehouse> Warehouses { get; set; } = new List<Warehouse>();
    }
}
