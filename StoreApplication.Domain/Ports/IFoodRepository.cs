using StoreApplication.Domain.Dtos;
using StoreApplication.Domain.Entities;

namespace StoreApplication.Domain.Ports;

public interface IFoodRepository
{
    Task AddProductAsync(Product product);
    Task UpdateProductAsync(Guid productId, string name, string description, decimal price, int stock);
    Task DeleteProductAsync(Guid productId);
    Task<PaginatedDto<Product>> GetAllProductsPagedAsync(int pageNumber, int pageSize, string? includeProperties = null);
}
