namespace StoreApplication.Domain.Dtos;
public class PaginatedDto<T> where T : class
{
    public IEnumerable<T> Data { get; set; } = default!;
    public int TotalPages { get; set; }
    public double TotalRecords { get; set; }
}
