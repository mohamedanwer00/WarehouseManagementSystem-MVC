using System;
using System.Collections.Generic;
using System.Text;

namespace WarehouseDAL.Entities
{
    public class Unit : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Symbol { get; set; } = null!;//الرمز
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
