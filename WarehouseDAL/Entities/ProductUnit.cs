using System;
using System.Collections.Generic;
using System.Text;

namespace WarehouseDAL.Entities
{
    public class ProductUnit : BaseEntity
    {
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int UnitId { get; set; }
        public Unit? Unit { get; set; }
        public int Factor { get; set; }
        public bool IsBaseUnit { get; set; }
        public decimal PurchasePrice { get; set; } // آخر سعر شراء للمنتج، ويتم تحديثه مع آخر فاتورة مشتريات.
        public decimal SellingPrice { get; set; }

    }
}
