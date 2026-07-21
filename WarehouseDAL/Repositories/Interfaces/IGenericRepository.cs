using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using WarehouseDAL.Entities;

namespace WarehouseDAL.Repositories.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : BaseEntity, new()
    {
        Task<TEntity?> GetById(int id);

        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        void AddRange(IEnumerable<TEntity> entity);
        void UpdateRange(IEnumerable<TEntity> entity);
        IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>>? condition = null);
        IQueryable<TEntity> GetTableNoTracking();
        IQueryable<TEntity> AsQueryable();
    }
}
