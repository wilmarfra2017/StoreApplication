using StoreApplication.Application.Dtos;
using StoreApplication.Domain.Entities;

namespace StoreApplication.Application.Mappers;

public static class ProductMapper
{    
    public static IEnumerable<ProductDto> ToProductDtos(IEnumerable<Product> products)
    {
        if (products == null)
            return new List<ProductDto>();

        return products.Select(product => new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock
        });
    }

    public static ProductDto ToProductDto(Product product)
    {
        if (product == null)
            return null!;

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock
        };
    }
}
