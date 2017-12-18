using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace NascoWebAPI.Data
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            int? skip = null, int? take = null, params Expression<Func<T, object>>[] includeProperties);

        Task<IEnumerable<T>> GetAllAsync(Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            int? skip = null, int? take = null, params Expression<Func<T, object>>[] includeProperties);

        IEnumerable<T> Get(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
             int? skip = null, int? take = null, params Expression<Func<T, object>>[] includeProperties);
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            int? skip = null, int? take = null, params Expression<Func<T, object>>[] includeProperties);
        T GetSingle(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includeProperties);
        Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includeProperties);
        T GetFirst(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, params Expression<Func<T, object>>[] includeProperties);
        Task<T> GetFirstAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, params Expression<Func<T, object>>[] includeProperties);
        //T GetById(object id);
        object Max(Expression<Func<T, bool>> predicate = null, Expression<Func<T, object>> selector= null);
        IRepository<TEntity> GetRepository<TEntity>() where TEntity: class;
        IEnumerable<T> SQLQuery(string sql, params object[] parameters);
        bool Any(Expression<Func<T, bool>> predicate = null);
        int Count(Expression<Func<T, bool>> predicate = null);

        void Insert(T entity);
        void Update(T entity);
        void Delete(T entity);
        void DeleteWhere(Expression<Func<T, bool>> predicate);
        void Unchanged(T entity);
        void Detached(T entity);

        int SaveChanges();
        void Dispose();

        IEnumerable<T> Include(params Expression<Func<T, object>>[] includeProperties);
    }
}
