namespace StoreApplication.Domain.Entities;
public record CartItem(
    Guid ProductId,
    int Quantity
);
