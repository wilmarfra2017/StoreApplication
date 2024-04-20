namespace StoreApplication.Api.Dtos;

public record CreateProductRequestDto(
   string Name,
   string Description,
   decimal Price,
   int Stock
);

