using StoreApplication.Domain.Entities;

namespace StoreApplication.Domain.Ports;
public interface IProductRepository
{
    Task<Product> GetProductByIdAsync(Guid productId);
    Task UpdateProductStockAsync(Guid productId, int quantity);
}
