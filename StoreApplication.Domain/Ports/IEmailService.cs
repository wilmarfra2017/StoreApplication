using StoreApplication.Domain.Entities;

namespace StoreApplication.Domain.Ports;

public interface IEmailService
{
    Task SendOrderConfirmationEmailAsync(string to, Order order);
}
