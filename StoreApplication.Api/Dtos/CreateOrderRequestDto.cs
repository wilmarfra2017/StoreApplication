namespace StoreApplication.Api.Dtos;

public class CreateOrderRequestDto
{
    public Guid UserId { get; set; }
    public List<OrderDetailDto> OrderDetails { get; set; } = default!;
    public string Email { get; set; } = default!;
}

public class OrderDetailDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
