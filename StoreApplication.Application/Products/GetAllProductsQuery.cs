using MediatR;
using StoreApplication.Application.Dtos;

namespace StoreApplication.Application.Products;

public class GetAllProductsQuery : IRequest<ResponsePaginatedDto<ProductDto>>
{
    public int Page { get; set; }
    public int PageSize { get; set; }

    public GetAllProductsQuery(int page, int pageSize)
    {
        Page = page;
        PageSize = pageSize;
    }
}
