using StoreApplication.Domain.Entities;
using StoreApplication.Domain.Ports;

namespace StoreApplication.Domain.Services;

[DomainService]
public class ProductAvailabilityService : IProductAvailabilityService
{
    private readonly IProductRepository _productRepository;

    public ProductAvailabilityService(IProductRepository productRepository)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    }

    public async Task CheckProductAvailabilityAsync(List<OrderDetail> orderDetails)
    {
        foreach (var detail in orderDetails)
        {
            var product = await _productRepository.GetProductByIdAsync(detail.ProductId);
            if (product == null)
            {
                throw new InvalidOperationException($"Product with ID {detail.ProductId} not found.");
            }
            if (product.Stock < detail.Quantity)
            {
                throw new InvalidOperationException($"Not enough stock for product {product.Name}. Available: {product.Stock}, Requested: {detail.Quantity}.");
            }
            
            await _productRepository.UpdateProductStockAsync(detail.ProductId, detail.Quantity);
        }
    }
}
