using MediatR;
using Microsoft.Extensions.Logging;
using StoreApplication.Application.FoodOrder;
using StoreApplication.Domain.Entities;
using StoreApplication.Domain.Ports;

namespace StoreApplication.Application.Products;

public class AddProductCommandHandler : IRequestHandler<AddProductCommand, Guid>
{
    private readonly IFoodRepository _foodRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddProductCommandHandler> _logger;
    private readonly ILogMessageService _resourceManager;

    public AddProductCommandHandler(IFoodRepository foodRepository, IUnitOfWork unitOfWork,
        ILogger<AddProductCommandHandler> logger, ILogMessageService resourceManager)
    {
        _foodRepository = foodRepository ?? throw new ArgumentNullException(nameof(foodRepository));
        _unitOfWork = unitOfWork;
        _logger = logger;
        _resourceManager = resourceManager;
    }

    public async Task<Guid> Handle(AddProductCommand request, CancellationToken cancellationToken)
    {
        _logger.Log(LogLevel.Information, "Method Handle - AddProductCommandHandler");

        try
        {
            var newProduct = new Product(
                request.Name,
                request.Description,
                request.Price,
                request.Stock
            );

            await _foodRepository.AddProductAsync(newProduct);
            await _unitOfWork.SaveAsync(cancellationToken);

            return newProduct.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError($"{_resourceManager.ErrorSavingChanges} {ex.InnerException?.Message}");
            throw;
        }
    }

}