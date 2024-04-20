using Microsoft.EntityFrameworkCore;
using StoreApplication.Domain.Entities;
using StoreApplication.Domain.Ports;
using StoreApplication.Infrastructure.DataSource;

namespace StoreApplication.Infrastructure.Adapters;

[Repository]
public class OrderRepository : IOrderRepository
{
    private readonly DataContext _context;
    private readonly IProductAvailabilityService _availabilityService;
    private readonly IEmailService _emailService;

    public OrderRepository(DataContext context, IProductAvailabilityService availabilityService, IEmailService emailService)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _availabilityService = availabilityService ?? throw new ArgumentNullException(nameof(availabilityService));
        _emailService = emailService;
    }

    public async Task<Order> PlaceOrderAsync(Guid userId, List<OrderDetail> orderDetails, string email)
    {
        using var transaction = _context.Database.BeginTransaction();
        try
        {
            await _availabilityService.CheckProductAvailabilityAsync(orderDetails);
            decimal total = CalculateTotal(orderDetails);

            var order = new Order
            {                
                UserId = userId,
                Email = email,
                OrderDate = DateTime.UtcNow,
                Total = total,
                CreatedOn = DateTime.UtcNow,
                LastModifiedOn = DateTime.UtcNow
            };

            await _context.AddAsync(order);

            var newOrderDetails = orderDetails.Select(od =>
            {
                var detail = new OrderDetail
                {
                    ProductId = od.ProductId,
                    Quantity = od.Quantity
                };
                
                detail.GetType().GetProperty("OrderId")?.SetValue(detail, order.Id);
                detail.GetType().GetProperty("UnitPrice")?.SetValue(detail, _context.Products.Find(od.ProductId)?.Price ?? default);
                detail.GetType().GetProperty("CreatedOn")?.SetValue(detail, DateTime.UtcNow);
                detail.GetType().GetProperty("LastModifiedOn")?.SetValue(detail, DateTime.UtcNow);

                return detail;
            }).ToList();

            foreach (var detail in newOrderDetails)
            {
                order.OrderDetails.Add(detail);
            }


            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            var completeOrder = await GetOrderWithDetailsAsync(order.Id);

            await _emailService.SendOrderConfirmationEmailAsync(email, completeOrder);

            return order;
        }
        catch (DbUpdateException ex)
        {
            await transaction.RollbackAsync();
            throw new Exception($"An error occurred while saving the entity changes. {ex.InnerException?.Message}", ex);
        }
    }



    private decimal CalculateTotal(List<OrderDetail> orderDetails)
    {
        decimal total = 0;
        foreach (var detail in orderDetails)
        {
            var product = _context.Products.Find(detail.ProductId);
            if (product != null)
            {
                total += product.Price * detail.Quantity;
            }
        }
        return total;
    }

    public async Task<Order> GetOrderWithDetailsAsync(Guid orderId)
    {
        var order = await _context.Orders
                                  .Include(o => o.OrderDetails)
                                  .SingleOrDefaultAsync(o => o.Id == orderId);

        if (order != null)
        {
            foreach (var detail in order.OrderDetails)
            {
                var product = await _context.Products.FindAsync(detail.ProductId);
                if (product != null)
                {
                    detail.ProductName = product.Name;
                }
            }
        }

        return order!;
    }
}
