using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreApplication.Api.Dtos;
using StoreApplication.Api.Mappers;
using StoreApplication.Application.Products;
using StoreApplication.Domain.Ports;

namespace StoreApplication.Api.Controllers;

[Route("api/[controller]")]
[ApiController]

public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IValidator<AddProductCommand> _productValidator;
    private readonly IValidator<UpdateProductCommand> _productUpdateValidator;
    private readonly ILogger<OrdersController> _logger;
    private readonly ILogMessageService _resourceManager;

    public ProductsController(IMediator mediator, IValidator<AddProductCommand> productValidator, 
        IValidator<UpdateProductCommand> productUpdateValidator, ILogger<OrdersController> logger,
        ILogMessageService resourceManager)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _productValidator = productValidator ?? throw new ArgumentNullException(nameof(productValidator));
        _productUpdateValidator = productUpdateValidator;
        _resourceManager = resourceManager;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Policy = "MustBeAdmin")]
    public async Task<IActionResult> AddProduct([FromBody] CreateProductRequestDto request)
    {
        _logger.Log(LogLevel.Information, "Method AddProduct - ProductsController");

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var command = request.ToAddProductCommand();
        var validationResult = await _productValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        try
        {
            var productId = await _mediator.Send(command);
            var result = new
            {
                Message = _resourceManager.ProductCreatedSuccess,
                OrderId = productId
            };

            return CreatedAtAction(nameof(GetProduct), new { id = productId }, result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public IActionResult GetProduct(Guid id)
    {
        return Ok();
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "MustBeAdmin")]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var command = request.ToUpdateProductCommand();        
        var validationResult = await _productUpdateValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        command.Id = id;
        try
        {
            var productId = await _mediator.Send(command);
            return Ok(productId); 
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "MustBeAdmin")]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        try
        {
            var command = new DeleteProductCommand(id);
            await _mediator.Send(command);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {            
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet]
    [Authorize(Policy = "AdminOrUser")]
    public async Task<IActionResult> GetAllProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (page < 1 || pageSize < 1)
        {
            return BadRequest(_resourceManager.ParamsPageGreaterZero);
        }

        try
        {            
            var query = new GetAllProductsQuery(page, pageSize);
            var result = await _mediator.Send(query);

            if (result == null || !result.Data.Any())
            {
                return NotFound(_resourceManager.NoProductsFound);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"{_resourceManager.InternalServerError}: {ex.Message}");
        }
    }
}
