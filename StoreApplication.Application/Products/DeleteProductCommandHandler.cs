using MediatR;
using Microsoft.Extensions.Logging;
using StoreApplication.Domain.Ports;

namespace StoreApplication.Application.Products;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly IFoodRepository _foodRepository;
    private readonly ILogger<DeleteProductCommandHandler> _logger;

    public DeleteProductCommandHandler(IFoodRepository foodRepository, ILogger<DeleteProductCommandHandler> logger)
    {
        _foodRepository = foodRepository ?? throw new ArgumentNullException(nameof(foodRepository));
        _logger = logger;
    }

    public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request), "DeleteProductCommand is null");
        }

        try
        {
            await _foodRepository.DeleteProductAsync(request.Id);
            return Unit.Value;
        }
        catch (KeyNotFoundException ex)
        {                        
            _logger.LogError($"The product with ID {request.Id} not found. Error: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error deleting product: " + ex.Message);            
            throw;
        }
    }
}
