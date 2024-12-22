using AutoRepairPro.Data.Repositories.Interfaces;
using Integration.data.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Integration.data.Repositories.Implementation
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _dbContext;

        public GenericRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
        }

        public async Task AddRange(List<T> entities)
        {
            await _dbContext.Set<T>().AddRangeAsync(entities);
        }

        public async Task<IEnumerable<T>> GetAllAsNoTracking(string[] InclueProperties = null)
        {
            IQueryable<T> Query = _dbContext.Set<T>().AsNoTracking().AsQueryable();
            if (InclueProperties != null)
            {
                foreach (var includeProperty in InclueProperties)
                {
                    Query = Query.Include(includeProperty.Trim());
                }
            }

            return await Query.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsTracking(string[] InclueProperties = null)
        {
            IQueryable<T> Query = _dbContext.Set<T>().AsQueryable();
            if (InclueProperties != null)
            {
                foreach (var includeProperty in InclueProperties)
                {
                    Query = Query.Include(includeProperty.Trim());
                }
            }

            return await Query.ToListAsync();
        }

        public async Task<T> GetFirstOrDefault(Expression<Func<T, bool>> filter, string[] InclueProperties = null)
        {
            IQueryable<T> Query = _dbContext.Set<T>().AsQueryable();
            Query = Query.Where(filter);
            if (InclueProperties != null)
            {
                foreach (var includeProperty in InclueProperties)
                {
                    Query = Query.Include(includeProperty.Trim());
                }
            }

            return await Query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsNoTracking(Expression<Func<T, bool>> filter, string[] InclueProperties = null)
        {
            IQueryable<T> Query = _dbContext.Set<T>().AsQueryable();
            Query = Query.Where(filter);
            if (InclueProperties != null)
            {
                foreach (var includeProperty in InclueProperties)
                {
                    Query = Query.Include(includeProperty.Trim());
                }
            }
            return await Query.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsTracking(Expression<Func<T, bool>> filter, string[] InclueProperties = null)
        {
            IQueryable<T> Query = _dbContext.Set<T>().AsQueryable();
            Query = Query.Where(filter);
            if (InclueProperties != null)
            {
                foreach (var includeProperty in InclueProperties)
                {
                    Query = Query.Include(includeProperty.Trim());
                }
            }
            return await Query.ToListAsync();
        }

        public void Remove(T Entity)
        {
            _dbContext.Set<T>().Remove(Entity);
        }

        public void Update(T entity)
        {
            _dbContext.Set<T>().Update(entity);
        }

        public async Task<bool> SaveChanges()
        {
            var RowsEfected = await _dbContext.SaveChangesAsync();
            return RowsEfected > 0 ? true : false;
        }

        public async Task<int> GetCount()
        {
            return await _dbContext.Set<T>().CountAsync();
        }

        public void RemoveRange(IEnumerable<T> Entities)
        {
            _dbContext.Set<T>().RemoveRange(Entities);
        }

        public IQueryable<T> GetAllQuerableAsNoTracking(string[] InclueProperties = null)
        {
            IQueryable<T> Query = _dbContext.Set<T>().AsNoTracking().AsQueryable();
            if (InclueProperties != null)
            {
                foreach (var includeProperty in InclueProperties)
                {
                    Query = Query.Include(includeProperty.Trim());
                }
            }
            return Query.AsQueryable();
        }
        public async Task<IQueryable<T>> GetAllQueryableAsNoTrackingAsync(string[] includeProperties = null, Expression<Func<T, bool>> predicate = null)
        {
            IQueryable<T> query = _dbContext.Set<T>().AsNoTracking();

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty.Trim());
                }
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            // Materialize the query asynchronously
            await query.ToListAsync();

            return query;
        }


        public async Task<bool> AnyAsync(Expression<Func<T, bool>> filter)
        {
            return await _dbContext.Set<T>().AnyAsync(filter);
        }
    }
}