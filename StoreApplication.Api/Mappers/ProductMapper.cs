using StoreApplication.Api.Dtos;
using StoreApplication.Application.Products;

namespace StoreApplication.Api.Mappers;

public static class ProductMapper
{
    public static AddProductCommand ToAddProductCommand(this CreateProductRequestDto createProductRequestDto)
    {
        ArgumentNullException.ThrowIfNull(createProductRequestDto, nameof(createProductRequestDto));

        return new AddProductCommand(
            createProductRequestDto.Name,
            createProductRequestDto.Description,
            createProductRequestDto.Price,
            createProductRequestDto.Stock
        );
    }

    public static UpdateProductCommand ToUpdateProductCommand(this UpdateProductRequestDto updateProductRequestDto)
    {
        ArgumentNullException.ThrowIfNull(updateProductRequestDto, nameof(updateProductRequestDto));

        return new UpdateProductCommand(
            updateProductRequestDto.Id,
            updateProductRequestDto.Name,
            updateProductRequestDto.Description,
            updateProductRequestDto.Price,
            updateProductRequestDto.Stock
        );
    }
}
