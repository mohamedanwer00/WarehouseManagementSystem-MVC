using WarehouseDAL.Data.Contexts;
using WarehouseDAL.Entities;
using WarehouseDAL.Repositories.Interfaces;

namespace WarehouseDAL.Repositories.Implememtation;

public class SalesInvoiceRepository : GenericRepository<SalesInvoice>, ISalesInvoiceRepository
{
    private readonly WarehouseDbContext _dbContext;

    public SalesInvoiceRepository(WarehouseDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
