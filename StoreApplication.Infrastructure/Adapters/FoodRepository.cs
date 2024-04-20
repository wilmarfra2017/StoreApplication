using StoreApplication.Domain.Dtos;
using StoreApplication.Domain.Entities;
using StoreApplication.Domain.Ports;
using StoreApplication.Infrastructure.Ports;
using System.Linq.Expressions;

namespace StoreApplication.Infrastructure.Adapters;

[Repository]
public class FoodRepository : IFoodRepository
{
    readonly IRepository<Product> _context;
    private readonly IUnitOfWork _unitOfWork;

    public FoodRepository(IRepository<Product> context, IUnitOfWork unitOfWork)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _unitOfWork = unitOfWork;
    }

    public async Task AddProductAsync(Product product)
    {
        if (product == null)
            throw new ArgumentNullException(nameof(product), "Product data must not be null");

        product.CreatedOn = DateTime.UtcNow;
        product.LastModifiedOn = DateTime.UtcNow;
        await _context.AddAsync(product);
    }

    public async Task UpdateProductAsync(Guid productId, string name, string description, decimal price, int stock)
    {
        var product = await _context.GetOneAsync(productId);
        if (product == null)
            throw new KeyNotFoundException($"No product found with ID {productId}");

        product.UpdateProduct(name, description, price, stock);
        await _unitOfWork.SaveAsync();
    }

    public async Task DeleteProductAsync(Guid productId)
    {
        var product = await _context.GetOneAsync(productId);
        if (product == null)
            throw new KeyNotFoundException($"No product found with ID {productId}");

        _context.Delete(product);
        await _unitOfWork.SaveAsync();
    }

    public async Task<PaginatedDto<Product>> GetAllProductsPagedAsync(int pageNumber, int pageSize, string? includeProperties = null)
    {
        try
        {            
            Expression<Func<Product, bool>> filter = _ => true;
         
            Func<IQueryable<Product>, IOrderedQueryable<Product>> orderBy = q => q.OrderBy(p => p.Name);

            var pagedProducts = await _context.GetManyByFilterPaginatedAsync(
                filter,
                orderBy,
                pageNumber,
                pageSize,
                includeProperties,
                isTracking: false);

            var totalRecords = await _context.GetTotalRecordsByFilterAsync(filter);

            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            return new PaginatedDto<Product>
            {
                Data = pagedProducts.ToList(),
                TotalRecords = (int)totalRecords,
                TotalPages = totalPages
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error getting paginated products: {ex.Message}", ex);
        }
    }
}
