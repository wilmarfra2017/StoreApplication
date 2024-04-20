namespace StoreApplication.Api.Dtos;
public record UpdateProductRequestDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    int Stock
);
