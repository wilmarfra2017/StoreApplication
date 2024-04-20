using StoreApplication.Domain.Entities;

namespace StoreApplication.Domain.Ports;

public interface IOrderRepository
{
    Task<Order> PlaceOrderAsync(Guid userId, List<OrderDetail> orderDetails, string email);
    Task<Order> GetOrderWithDetailsAsync(Guid orderId);
}
