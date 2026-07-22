using WarehouseDAL.Data.Contexts;
using WarehouseDAL.Entities;
using WarehouseDAL.Repositories.Interfaces;

namespace WarehouseDAL.Repositories.Implememtation;

public class ProductWarehouseRepository:GenericRepository<ProductWarehouse>,IProductWarehouseRepository
{
    private readonly WarehouseDbContext _dbContext;

    public ProductWarehouseRepository(WarehouseDbContext dbContext): base(dbContext)
    {
        _dbContext = dbContext;
    }
}
