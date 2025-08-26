using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using shorten.url.application.Contracts;
using shorten.url.application.Models;
using shorten.url.domain;
using System.Data.Common;
using System.Data;
using System.Linq.Expressions;

namespace shorten.url.infrastructure.Persistence
{
    public class GenericRepository : IRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public GenericRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected virtual IQueryable<T> GetQueryable<T>(Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] includes) where T : BaseEntity, new()
        {
            IQueryable<T> query = _dbContext.Set<T>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            query = query.Where(x => 1 == 1);

            if (includes != null && includes.Any())
            {
                query = includes.Aggregate(query, (current, property) => current.Include(property));
            }

            return query;
        }

        protected virtual IQueryable<T> GetGenericQueryable<T>(Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] includes) where T : class, new()
        {
            IQueryable<T> query = _dbContext.Set<T>();
            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includes != null && includes.Any())
            {
                query = includes.Aggregate(query, (current, property) => current.Include(property));
            }
            return query;
        }


        public virtual async Task<T> GetByIdAsync<T>(Guid id) where T : BaseEntity, new()
        {
            return await GetByIdAsync<T>(id, null);
        }

        public virtual async Task<T> GetByIdAsync<T>(Guid id, params Expression<Func<T, object>>[] includes) where T : BaseEntity, new()
        {
            return await GetQueryable<T>(x => x.Id == id, includes).FirstOrDefaultAsync();
        }

        public virtual async Task<T> GetEntityAsync<T>(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includeProperties) where T : BaseEntity, new()
        {
            return await GetQueryable<T>(filter, includeProperties).FirstOrDefaultAsync();
        }  

        public async Task<T> AddAsync<T>(T entity) where T : class, new()
        {
            await _dbContext.Set<T>().AddAsync(entity);
            return entity;
        }

        public void Update<T>(T entity) where T : BaseEntity, new()
        {
            var dbSet = _dbContext.Set<T>();

            if (IsDetached(entity))
            {
                //dbSet.Attach(entity);
            }
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        private bool IsDetached<T>(T entity) where T : BaseEntity, new()
        {
            var dbSet = _dbContext.Set<T>();
            var localEntity = dbSet.Local?.FirstOrDefault(x => Equals(x.Id, entity.Id));
            if (localEntity != null) // entity stored in local
            {
                _dbContext.Entry(localEntity).State = EntityState.Detached;
                return false;
            }

            return _dbContext.Entry(entity).State == EntityState.Detached;
        }

        public void Delete<T>(T entity) where T : BaseEntity, new()
        {
            _dbContext.Set<T>().Remove(entity);
        }        

        public virtual IQueryable<T> ListQueryable<T>(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includeProperties) where T : BaseEntity, new()
        {
            return GetQueryable(filter, includeProperties);
        }

        public virtual IQueryable<T> ListGenericQueryable<T>(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includeProperties) where T : class, new()
        {
            return GetGenericQueryable(filter, includeProperties);
        }

        public virtual async Task<IReadOnlyList<T>> FromQuery<T>(string sqlQuery, object[] parameters, params Expression<Func<T, object>>[] includeProperties) where T : BaseEntity, new()
        {
            var query = _dbContext.Set<T>().FromSqlRaw(sqlQuery, parameters);

            query = query.Where(x => 1 == 1);

            if (includeProperties != null && includeProperties.Any())
            {
                query = includeProperties.Aggregate(query, (current, property) => current.Include(property));
            }

            return (IReadOnlyList<T>)await query.ToListAsync();
        }        

        public virtual async Task ExecuteNonQueryAsync(string sqlQuery, params QueryParameter[] parameters)
        {
            var paramList = new List<DbParameter>();
            foreach (var param in parameters)
            {
                paramList.Add(new SqlParameter(param.ParameterName, param.ParameterValue));
            }

            using (var command = _dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sqlQuery;
                command.CommandType = CommandType.Text;

                if (parameters != null)
                {
                    command.Parameters.AddRange(paramList.ToArray());
                }
                await _dbContext.Database.OpenConnectionAsync();
                await command.ExecuteNonQueryAsync();
                await _dbContext.Database.CloseConnectionAsync();
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
