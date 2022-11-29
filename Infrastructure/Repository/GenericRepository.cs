
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {

        internal ApplicationDbContext _dbContext;
        internal DbSet<T> table = null;
        public GenericRepository(ApplicationDbContext context)
        {
            _dbContext = context;
            table = _dbContext.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate = null,
               Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
               Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
               bool disableTracking = true
           )
        {
            IQueryable<T> query = table;
            if (disableTracking)
            {
                query = query.AsNoTracking();
            }
            if (include != null)
            {
                query = include(query);
            }
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }
            else
            {
                return await query.ToListAsync();
            }
        }
        public IQueryable<T> GetCustomAll(Expression<Func<T, bool>> predicate = null)
        {
            IQueryable<T> query = table;
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            return query;
        }

        public async Task<T> GetAsync(
               Expression<Func<T, bool>> predicate = null,
               Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
               bool disableTracking = true
           )
        {
            IQueryable<T> query = table;
            if (disableTracking)
            {
                query = query.AsNoTracking();
            }
            if (include != null)
            {
                query = include(query);
            }
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            return await query.FirstOrDefaultAsync();
        }
       

        public virtual async Task<T> FindByIdAsync(object id)
        {
            return await table.FindAsync(id);
        }

        public async Task<T> InsertAsync(T entity)
        {
            await table.AddAsync(entity);
            return entity;
        }

        public async Task<List<T>> InsertRangeAsync(List<T> entity)
        {
            await table.AddRangeAsync(entity);
            return entity;
        }

        public void Update(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        public void UpdateRange(List<T> entity)
        {
            _dbContext.UpdateRange(entity);
        }

        public void Delete(T entity)
        {
            table.Remove(entity);
        }
        public void DeleteRange(List<T> entity)
        {
            table.RemoveRange(entity);
        }

    }
}