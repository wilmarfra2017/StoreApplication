using MediatR;
using StoreApplication.Domain.Entities;

namespace StoreApplication.Application.FoodOrder;

public class PlaceOrderCommand : IRequest<Guid>
{
    public Guid UserId { get; }
    public List<OrderDetail> OrderDetails { get; }
    public string Email {  get; }

    public PlaceOrderCommand(Guid userId, List<OrderDetail> orderDetails, string email)
    {
        UserId = userId;
        OrderDetails = orderDetails ?? throw new ArgumentNullException(nameof(orderDetails));
        Email = email;
    }
}
