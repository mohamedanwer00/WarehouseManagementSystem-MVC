using System;
using System.Collections.Generic;
using System.Text;

namespace WarehouseDAL.Entities
{
    public class Product : BaseEntity
    {

        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
        public decimal MinimumQuantity { get; set; }
        public decimal MaximumQuantity { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public ICollection<ProductUnit> ProductUnits { get; set; } = new List<ProductUnit>();
        public ICollection<ProductWarehouse> ProductWarehouses { get; set; } = [];
    }
}
