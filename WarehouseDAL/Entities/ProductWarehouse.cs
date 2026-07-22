namespace WarehouseDAL.Entities;

public class ProductWarehouse : BaseEntity//المخزون هيتخزن فيه 
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;

    public decimal Quantity { get; set; }
}
