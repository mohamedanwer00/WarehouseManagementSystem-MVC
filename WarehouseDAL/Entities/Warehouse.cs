namespace WarehouseDAL.Entities;

public class Warehouse : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public int BranchId { get; set; }
    public Branch Branch { get; set; } = null!;

    public ICollection<OpeningStock> Stocks { get; set; } = [];
    public ICollection<ProductWarehouse> ProductWarehouses { get; set; } = [];

}
public class OpeningStock : BaseEntity
{
    public int WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public decimal Quantity { get; set; }
}
