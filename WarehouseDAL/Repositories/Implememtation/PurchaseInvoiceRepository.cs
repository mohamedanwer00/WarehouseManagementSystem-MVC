using System;
using System.Collections.Generic;
using System.Text;
using WarehouseDAL.Data.Contexts;
using WarehouseDAL.Entities;
using WarehouseDAL.Repositories.Interfaces;

namespace WarehouseDAL.Repositories.Implememtation
{
    public class PurchaseInvoiceRepository:GenericRepository<PurchaseInvoice>,IPurchaseInvoiceRepository
    {
        private readonly WarehouseDbContext _dbContext;

        public PurchaseInvoiceRepository(WarehouseDbContext dbContext):base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
