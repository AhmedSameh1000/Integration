using System.Linq.Expressions;


namespace AutoRepairPro.Data.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsTracking(string[] InclueProperties = null);

        //Task<IPagedList<T>> GetAllAsTracking(RequestParams requestParams, string[] InclueProperties = null);

        Task<IEnumerable<T>> GetAllAsNoTracking(string[] InclueProperties = null);

        Task<T> GetFirstOrDefault(Expression<Func<T, bool>> filter, string[] InclueProperties = null);

        Task<IEnumerable<T>> GetAllAsNoTracking(Expression<Func<T, bool>> filter, string[] InclueProperties = null);

        IQueryable<T> GetAllQuerableAsNoTracking(string[] InclueProperties = null);

        Task<IEnumerable<T>> GetAllAsTracking(Expression<Func<T, bool>> filter, string[] InclueProperties = null);
        public Task<IQueryable<T>> GetAllQueryableAsNoTrackingAsync(string[] includeProperties = null, Expression<Func<T, bool>> predicate = null);
        Task Add(T entity);

        Task AddRange(List<T> entities);

        void Update(T entity);

        Task<int> GetCount();

        void Remove(T Entity);

        void RemoveRange(IEnumerable<T> Entities);

        Task<bool> SaveChanges();
        Task<bool> AnyAsync(Expression<Func<T, bool>> filter);
    }
}
