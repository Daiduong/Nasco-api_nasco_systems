using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace NascoWebAPI.Data
{
    public class Repository<T> : IRepository<T>, IDisposable
            where T : class
    {

        protected ApplicationDbContext _context;
        private bool _disposed;
        #region Properties
        public Repository(ApplicationDbContext context)
        {
            _context = context;
        }
        protected virtual IQueryable<T> GetQueryable(Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            int? skip = null, int? take = null, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }
            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }
            return query;
        }
        #endregion
        public virtual IEnumerable<T> GetAll(Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            int? skip = null, int? take = null, params Expression<Func<T, object>>[] includeProperties)
        {
            return GetQueryable(null, orderBy, skip, take, includeProperties).ToList();
        }
        public virtual async Task<IEnumerable<T>> GetAllAsync(Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
    int? skip = null, int? take = null, params Expression<Func<T, object>>[] includeProperties)
        {
            return await GetQueryable(null, orderBy, skip, take, includeProperties).ToListAsync();
        }

        public virtual IEnumerable<T> Get(Expression<Func<T, bool>> predicate = null,
             Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
             int? skip = null, int? take = null, params Expression<Func<T, object>>[] includeProperties
            )
        {
            return GetQueryable(predicate, orderBy, skip, take, includeProperties).ToList();
        }
        public virtual async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            int? skip = null, int? take = null, params Expression<Func<T, object>>[] includeProperties)
        {
            return await GetQueryable(predicate, orderBy, skip, take, includeProperties).ToListAsync();
        }

        public virtual T GetSingle(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includeProperties)
        {
            return GetQueryable(predicate, null, null, null, includeProperties).SingleOrDefault();
        }

        public virtual async Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includeProperties)
        {
            return await GetQueryable(predicate, null, null, null, includeProperties).SingleOrDefaultAsync();
        }

        public virtual T GetFirst(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            params Expression<Func<T, object>>[] includeProperties)
        {
            return GetQueryable(predicate, orderBy, null, null, includeProperties).FirstOrDefault();
        }

        public virtual async Task<T> GetFirstAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            params Expression<Func<T, object>>[] includeProperties)
        {
            return await GetQueryable(predicate, orderBy, null, null, includeProperties).FirstOrDefaultAsync();
        }


        public virtual bool Any(Expression<Func<T, bool>> predicate = null)
        {
            return GetQueryable(predicate).Any();
        }

        public virtual int Count(Expression<Func<T, bool>> predicate = null)
        {
            return GetQueryable(predicate).Count();
        }

        public virtual object Max(Expression<Func<T, bool>> predicate = null, Expression<Func<T, object>> selector = null)
        {
            return GetQueryable(predicate, null, null, null, null).Max(selector);
        }
        //public virtual object Min(Expression<Func<T, bool>> predicate = null, Expression<Func<T, object>> includeProperty = null)
        //{
        //    return GetQueryable(predicate, null, new[] { includeProperty }, null, null).Min();
        //}
        //public virtual object Sum(Expression<Func<T, bool>> predicate = null, Expression<Func<T, object>> includeProperty = null)
        //{if (object)
        //    return GetQueryable(predicate, null, new[] { includeProperty }, null, null).Sum();
        //}

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            var type = typeof(TEntity).Name;
            var repositoryType = typeof(Repository<>);
            var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), _context);
            return (IRepository<TEntity>)repositoryInstance;
        }

        #region Modify
        public virtual void Insert(T entity)
        {
            Insert(entity, null);
        }
        public virtual void Insert(T entity, int? itemCreatedBy)
        {
            EntityEntry dbEntityEntry = _context.Entry<T>(entity);
            _context.Set<T>().Add(entity);
        }
        public virtual void Update(T entity)
        {
            Update(entity, null);
        }
        public virtual void Update(T entity, int? itemModifiedBy)
        {
            EntityEntry dbEntityEntry = _context.Entry<T>(entity);
            dbEntityEntry.State = EntityState.Modified;
        }
        public virtual void Delete(T entity)
        {
            EntityEntry dbEntityEntry = _context.Entry<T>(entity);
            dbEntityEntry.State = EntityState.Deleted;
        }

        public virtual void DeleteWhere(Expression<Func<T, bool>> predicate)
        {
            IEnumerable<T> entities = _context.Set<T>().Where(predicate);

            foreach (var entity in entities)
            {
                _context.Entry<T>(entity).State = EntityState.Deleted;
            }
        }
        public IEnumerable<T> SQLQuery(string sql, params object[] parameters)
        {
            return _context.Set<T>().FromSql(sql, parameters);
        }
        public virtual int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                if (this._context != null)
                {
                    this._context.Dispose();
                    this._context = null;
                }
            }
        }
        #endregion

        public virtual IEnumerable<T> Include(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }
            return query;
        }

    }
}
