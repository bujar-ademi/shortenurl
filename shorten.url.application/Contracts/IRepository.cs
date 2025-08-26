using shorten.url.domain;
using System.Linq.Expressions;
using shorten.url.application.Models;

namespace shorten.url.application.Contracts
{
    public interface IRepository
    {
        Task<T> GetByIdAsync<T>(Guid id) where T : BaseEntity, new();
        Task<T> GetByIdAsync<T>(Guid id, params Expression<Func<T, object>>[] includes) where T : BaseEntity, new();
        Task<T> GetEntityAsync<T>(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includeProperties) where T : BaseEntity, new();        
        Task<T> AddAsync<T>(T entity) where T : class, new();
        void Update<T>(T entity) where T : BaseEntity, new();
        void Delete<T>(T entity) where T : BaseEntity, new();
        IQueryable<T> ListQueryable<T>(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includeProperties) where T : BaseEntity, new();
        IQueryable<T> ListGenericQueryable<T>(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includeProperties) where T : class, new();
        Task<IReadOnlyList<T>> FromQuery<T>(string sqlQuery, object[] parameters, params Expression<Func<T, object>>[] includeProperties) where T : BaseEntity, new();
        Task ExecuteNonQueryAsync(string sqlQuery, params QueryParameter[] parameters);        
        Task<int> SaveChangesAsync();
    }
}
