using MediatR;
using Microsoft.Extensions.Logging;
using StoreApplication.Application.Admins;
using StoreApplication.Domain.Ports;

namespace StoreApplication.Application.FoodOrder;

public class PlaceOrderHandler : IRequestHandler<PlaceOrderCommand, Guid>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductAvailabilityService _availabilityService;
    private readonly ILogger<PlaceOrderHandler> _logger;
    
    public PlaceOrderHandler(IOrderRepository orderRepository, IProductAvailabilityService availabilityService,
        ILogger<PlaceOrderHandler> logger)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _availabilityService = availabilityService ?? throw new ArgumentNullException(nameof(availabilityService));
        _logger = logger;        
    }

    public async Task<Guid> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        _logger.Log(LogLevel.Information, "Method Handle - PlaceOrderHandler");

        await _availabilityService.CheckProductAvailabilityAsync(request.OrderDetails);
        
        var order = await _orderRepository.PlaceOrderAsync(request.UserId, request.OrderDetails, request.Email);

        return order.Id;
    }
}
