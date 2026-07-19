using System;
using System.Collections.Generic;
using System.Text;

namespace WarehouseDAL.Entities
{
    public class Customer:BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Address { get; set; }=null!;
        public string PhoneNumber { get; set; } = null!;
        public decimal OpeningBalance { get; set; }
        public decimal CurrentBalance { get; set; }
    }
}
