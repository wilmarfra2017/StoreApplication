using MediatR;

namespace StoreApplication.Application.Products;

public class DeleteProductCommand : IRequest
{
    public Guid Id { get; private set; }

    public DeleteProductCommand(Guid id)
    {
        Id = id;
    }
}
