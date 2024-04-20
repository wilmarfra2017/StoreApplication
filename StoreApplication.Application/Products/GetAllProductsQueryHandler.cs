using MediatR;
using StoreApplication.Application.Dtos;
using StoreApplication.Application.Mappers;
using StoreApplication.Domain.Ports;

namespace StoreApplication.Application.Products;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, ResponsePaginatedDto<ProductDto>>
{
    private readonly IFoodRepository _foodRepository;

    public GetAllProductsQueryHandler(IFoodRepository foodRepository)
    {
        _foodRepository = foodRepository;
    }

    public async Task<ResponsePaginatedDto<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        
        var paginatedProducts = await _foodRepository.GetAllProductsPagedAsync(request.Page, request.PageSize);

        var productDtos = ProductMapper.ToProductDtos(paginatedProducts.Data);

        return new ResponsePaginatedDto<ProductDto>
        {
            Data = productDtos,
            TotalRecords = (int)paginatedProducts.TotalRecords,
            TotalPages = paginatedProducts.TotalPages,
            CurrentPage = request.Page,
            PageSize = request.PageSize
        };
    }
}
