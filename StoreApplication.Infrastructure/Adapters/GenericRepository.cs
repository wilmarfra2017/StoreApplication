using Microsoft.EntityFrameworkCore;
using StoreApplication.Domain.Entities;
using StoreApplication.Infrastructure.DataSource;
using StoreApplication.Infrastructure.Ports;
using System.Linq.Expressions;

namespace StoreApplication.Infrastructure.Adapters;

public class GenericRepository<T> : IRepository<T> where T : DomainEntity
{

    private readonly DbSet<T> _dataset;

    public GenericRepository(DataContext context)
    {
        var localContext = context ?? throw new ArgumentNullException(nameof(context));
        _dataset = localContext.Set<T>();
    }

    public async Task<T?> GetOneAsync(Guid id)
    {

        if (id == Guid.Empty)
        {
            throw new ArgumentException("Invalid ID", nameof(id));
        }

        return await _dataset.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetManyAsync()
    {
        return await _dataset.ToListAsync();
    }

    public async Task<IEnumerable<T>> GetManyAsync(Expression<Func<T, bool>> filter)
    {
        return await _dataset.Where(filter).ToListAsync();
    }

    public async Task<IEnumerable<T>> GetManyAsync(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy)
    {
        var query = _dataset.Where(filter);
        return await orderBy(query).ToListAsync();
    }

    public async Task<IEnumerable<T>> GetManyAsync(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy, string includeStringProperties)
    {
        var query = _dataset.Where(filter).Include(includeStringProperties);
        return await orderBy(query).ToListAsync();
    }

    public async Task<IEnumerable<T>> GetManyAsync(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy, string includeStringProperties, bool isTracking)
    {
        var query = isTracking ? _dataset.Where(filter).Include(includeStringProperties) : _dataset.AsNoTracking().Where(filter).Include(includeStringProperties);
        return await orderBy(query).ToListAsync();
    }

    public async Task<T> AddAsync(T entity)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity), "Entity can not be null");
        await _dataset.AddAsync(entity);
        return entity;
    }

    public void Delete(T entity)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity), "Entity can not be null");
        _dataset.Remove(entity);
    }

    public void Update(T entity)
    {
        _dataset.Update(entity);
    }

    public async Task<IEnumerable<T>> GetManyByFilterPaginatedAsync(
        Expression<Func<T, bool>> filter,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy,
        int pageNumber,
        int pageSize,
        string? includeStringProperties = null,
        bool isTracking = false)
    {
        IQueryable<T> query = _dataset.Where(filter);

        if (!isTracking)
        {
            query = query.AsNoTracking();
        }

        if (!string.IsNullOrEmpty(includeStringProperties))
        {
            foreach (var includeProperty in includeStringProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        return await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
    }

    public async Task<double> GetTotalRecordsByFilterAsync(Expression<Func<T, bool>> filter)
    {
        return await _dataset.Where(filter).CountAsync();
    }
}
