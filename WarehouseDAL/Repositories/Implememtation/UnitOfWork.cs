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
        public IBranchRepository Branches { get; } // 1. تعريف الـ Property هنا

        // 2. تمرير الـ IBranchRepository عبر الـ Constructor وحقنه
        public UnitOfWork(
            WarehouseDbContext dbContext,
            ICategoryRepository categories,
            IBranchRepository branches)
        {
            _dbContext = dbContext;
            Categories = categories;
            Branches = branches; // 3. إسناد القيمة الممررة للـ Property
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