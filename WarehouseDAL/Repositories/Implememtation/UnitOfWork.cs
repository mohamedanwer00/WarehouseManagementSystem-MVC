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

        public UnitOfWork(
            WarehouseDbContext dbContext,
            ICategoryRepository categories,
            IBranchRepository branches,
            IWarehouseRepository warehouses)
        {
            _dbContext = dbContext;
            Categories = categories;
            Branches = branches;
            Warehouses = warehouses;
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