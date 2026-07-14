using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using WarehouseDAL.Data.Contexts;
using WarehouseDAL.Entities;
using WarehouseDAL.Repositories.Interfaces;

namespace WarehouseDAL.Repositories.Implememtation
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity, new()
    {
        private readonly WarehouseDbContext _dbContext;

        public GenericRepository(WarehouseDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(TEntity entity)
        {
            _dbContext.Set<TEntity>().Add(entity);
        }

        public void Delete(TEntity entity)
        {
            _dbContext.Set<TEntity>().Remove(entity);
        }

        // 1. الدالة الأساسية المعدلة: ترجع IEnumerable مع إمكانية التصفية (Filter)
        public IEnumerable<TEntity> GetAll(Func<TEntity, bool>? condition = null)
        {
            if (condition is null)
                return _dbContext.Set<TEntity>().AsNoTracking().ToList();
            else
                return _dbContext.Set<TEntity>().AsNoTracking().Where(condition).ToList();
        }

        // 2. الدالة السحرية الجديدة: تتيح لك عمل Include و Querying مرن ومباشر من الـ Controller
        public IQueryable<TEntity> GetTableNoTracking()
        {
            return _dbContext.Set<TEntity>().AsNoTracking();
        }

        public TEntity? GetById(int id) => _dbContext.Set<TEntity>().Find(id);

        public void Update(TEntity entity)
        {
            _dbContext.Set<TEntity>().Update(entity);
        }
    }
}