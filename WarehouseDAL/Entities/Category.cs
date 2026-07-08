using System;
using System.Collections.Generic;
using System.Text;
namespace WarehouseDAL.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = null!;

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
