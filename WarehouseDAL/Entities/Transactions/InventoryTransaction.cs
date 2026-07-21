namespace WarehouseDAL.Entities.Transactions
{
    //معاملة جرد المخزن
    public class InventoryTransaction : BaseEntity
    {
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int BranchId { get; set; }
        public Branch Branch { get; set; } = null!;

        public int WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; } = null!;

        public InventoryTransactionType InventoryTransactionType { get; set; }

        public string? ReferenceNumber { get; set; }

        public DateTime Date { get; set; }

        public decimal Quantity { get; set; }
    }

    public enum InventoryTransactionType
    {
        Purchase = 1,//شراء 
        Sell = 2,//بيع
        OpeningStock = 3 //مخزون افتتاحى 

        //Transfer = 4 // نقل من مخزن لمخزن
    }
}
