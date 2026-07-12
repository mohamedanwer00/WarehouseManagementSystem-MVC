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


        //public int BaseUnitId { get; set; }
        //public Unit BaseUnit { get; set; } = null!;



        //public string Name { get; set; } = null!;
        //public string Code { get; set; } = null!;
        //public string Image { get; set; } = null!;
        //public decimal PurchasePrice { get; set; }//سعر الشراء
        //public decimal SellingPrice { get; set; }//سعر البيع
        //public decimal WholesalePrice { get; set; }//سعر الجملة
        //public int MinimumQuantity { get; set; }
        //public int MaximumQuantity { get; set; }
        //public int CurrentQuantity { get; set; }//الكمية الحالية
        //public string? Color { get; set; }

        //public string? Description { get; set; }
        //public int CategoryId { get; set; }
        //public Category Category { get; set; } = null!;
        //public int UnitId { get; set; }
        //public Unit Unit { get; set; } = null!;
    }
}
