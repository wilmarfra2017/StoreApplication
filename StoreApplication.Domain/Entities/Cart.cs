namespace StoreApplication.Domain.Entities;
public record Cart(
    Guid UserId,
    List<CartItem> Items
);