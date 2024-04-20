namespace StoreApplication.Application.Dtos;

public class ResponsePaginatedDto<T>
{
    public IEnumerable<T> Data { get; set; } = default!;
    public int TotalRecords { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }

    public ResponsePaginatedDto() { }


    public ResponsePaginatedDto(IEnumerable<T> data, int totalRecords, int totalPages, int currentPage, int pageSize)
    {
        Data = data;
        TotalRecords = totalRecords;
        TotalPages = totalPages;
        CurrentPage = currentPage;
        PageSize = pageSize;
    }
}
