using Microsoft.EntityFrameworkCore;
using StoreApplication.Domain.Entities;
using StoreApplication.Domain.Ports;
using StoreApplication.Infrastructure.DataSource;

namespace StoreApplication.Infrastructure.Adapters;

[Repository]
public class ProductRepository : IProductRepository
{
    private readonly DataContext _context;

    public ProductRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<Product> GetProductByIdAsync(Guid productId)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
        if (product == null)
        {
            throw new InvalidOperationException($"Product with ID {productId} not found.");
        }
        return product;
    }

    public async Task UpdateProductStockAsync(Guid productId, int quantity)
    {
        var product = await GetProductByIdAsync(productId);
        if (product == null)
        {
            throw new InvalidOperationException($"Product with ID {productId} not found.");
        }

        product.Stock -= quantity;
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }
}
