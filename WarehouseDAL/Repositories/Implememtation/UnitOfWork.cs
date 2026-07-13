using System;
using System.Collections.Generic;
using System.Text;
using WarehouseDAL.Data.Contexts;
using WarehouseDAL.Repositories.Interfaces;

namespace WarehouseDAL.Repositories.Implememtation
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly WarehouseDbContext _dbContext;

        public ICategoryRepository Categories { get; }
        public IBranchRepository Branches { get; }
        public IWarehouseRepository Warehouses { get; }
        public ICashBoxRepository CashBoxes { get; }

        public UnitOfWork(
            WarehouseDbContext dbContext,
            ICategoryRepository categories,
            IBranchRepository branches,
            IWarehouseRepository warehouses,
            ICashBoxRepository cashBoxes)
        {
            _dbContext = dbContext;
            Categories = categories;
            Branches = branches;
            Warehouses = warehouses;
            CashBoxes = cashBoxes;
        }

        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}