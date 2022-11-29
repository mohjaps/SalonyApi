using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace Core.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        /// <summary>
        /// Get multiple rows
        /// </summary>
        /// <param name="predicate"> like (predicate: b => b.Id == "fa7427fb-d948-4bc4-9398-31a02fd2bca1") </param>
        /// <param name="orderBy"> like (orderBy: source => source.orderBy(b => b.id).ThenBy(b=>b.fullName))</param>
        /// <param name="include"> like (include: source => source.Include(b => b.clientOrders).ThenInclude(b=>b.FK_Provider)) </param>
        /// <param name="disableTracking"> true or false </param>
        /// <returns> list </returns>
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate = null,
                                        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                        Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
                                        bool disableTracking = true
                                        );

        IQueryable<T> GetCustomAll(Expression<Func<T, bool>> predicate = null);
        /// <summary>
        /// Get first row
        /// </summary>
        /// <param name="predicate"> like (predicate: b => b.Id == "fa7427fb-d948-4bc4-9398-31a02fd2bca1") </param>
        /// <param name="include"> like (include: source => source.Include(b => b.clientOrders).ThenInclude(b=>b.FK_Provider)) </param>
        /// <param name="disableTracking"> true or false </param>
        /// <returns> object </returns>

        Task<T> GetAsync(Expression<Func<T, bool>> predicate = null,
                                        Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
                                        bool disableTracking = true
                                        );


        Task<T> FindByIdAsync(object id);
        Task<T> InsertAsync(T entity);
        Task<List<T>> InsertRangeAsync(List<T> entity);
        void Update(T entity);
        void UpdateRange(List<T> entity);
        void Delete(T entity);
        void DeleteRange(List<T> entity);

    }
}