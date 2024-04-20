using StoreApplication.Domain.Entities;
using System.Linq.Expressions;

namespace StoreApplication.Infrastructure.Ports;

public interface IRepository<T> where T : DomainEntity
{
    Task<T?> GetOneAsync(Guid id);

    Task<IEnumerable<T>> GetManyAsync();
    Task<IEnumerable<T>> GetManyAsync(Expression<Func<T, bool>> filter);
    Task<IEnumerable<T>> GetManyAsync(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy);
    Task<IEnumerable<T>> GetManyAsync(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy, string includeStringProperties);
    Task<IEnumerable<T>> GetManyAsync(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy, string includeStringProperties, bool isTracking);
    Task<IEnumerable<T>> GetManyByFilterPaginatedAsync(
        Expression<Func<T, bool>> filter,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy,
        int pageNumber,
        int pageSize,
        string? includeStringProperties = null,
        bool isTracking = false);
    Task<double> GetTotalRecordsByFilterAsync(Expression<Func<T, bool>> filter);

    Task<T> AddAsync(T entity);

    void Update(T entity);
    void Delete(T entity);
}