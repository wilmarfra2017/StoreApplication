using StoreApplication.Domain.Entities;

namespace StoreApplication.Domain.Ports;
public interface IProductAvailabilityService
{
    Task CheckProductAvailabilityAsync(List<OrderDetail> orderDetails);
}
