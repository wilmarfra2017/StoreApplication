using MediatR;
using StoreApplication.Domain.Ports;

namespace StoreApplication.Application.Products;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Guid>
{
    private readonly IFoodRepository _foodRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductCommandHandler(IFoodRepository foodRepository, IUnitOfWork unitOfWork)
    {
        _foodRepository = foodRepository ?? throw new ArgumentNullException(nameof(foodRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Guid> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _foodRepository.UpdateProductAsync(request.Id, request.Name, request.Description, request.Price, request.Stock);
            await _unitOfWork.SaveAsync(cancellationToken);
            return request.Id;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error updating product: " + ex.InnerException?.Message);
            throw;
        }
    }
}