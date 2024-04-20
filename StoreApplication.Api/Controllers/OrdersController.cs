using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreApplication.Api.Dtos;
using StoreApplication.Application.FoodOrder;
using StoreApplication.Domain.Entities;
using StoreApplication.Domain.Ports;

namespace StoreApplication.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IValidator<PlaceOrderCommand> _placeOrderValidator;
    private readonly ILogger<OrdersController> _logger;
    private readonly ILogMessageService _resourceManager;

    public OrdersController(IMediator mediator, IValidator<PlaceOrderCommand> placeOrderValidator, ILogger<OrdersController> logger,
        ILogMessageService resourceManager)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _placeOrderValidator = placeOrderValidator ?? throw new ArgumentNullException(nameof(placeOrderValidator));
        _logger = logger;
        _resourceManager = resourceManager;
    }

    [HttpPost]
    [Authorize(Policy = "MustBeUser")]
    public async Task<IActionResult> PlaceOrder([FromBody] CreateOrderRequestDto request)
    {
        _logger.Log(LogLevel.Information, "Method PlaceOrder - OrdersController");

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var orderDetails = request.OrderDetails.Select(d => OrderDetail.CreateForOrder(
            d.ProductId,
            d.Quantity
        )).ToList();

        var command = new PlaceOrderCommand(request.UserId, orderDetails, request.Email);
        var validationResult = await _placeOrderValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        try
        {
            var orderId = await _mediator.Send(command);
            var result = new
            {
                Message = _resourceManager.OrderCreatedSuccess,
                OrderId = orderId
            };
            return CreatedAtAction(nameof(GetOrder), new { id = orderId }, result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [HttpGet("{id}")]
    [Authorize(Policy = "MustBeUser")]
    public IActionResult GetOrder(Guid id)
    {        
        return Ok();
    }
}
