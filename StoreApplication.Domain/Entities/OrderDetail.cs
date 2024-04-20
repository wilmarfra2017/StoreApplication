using System.ComponentModel.DataAnnotations.Schema;

namespace StoreApplication.Domain.Entities;

public class OrderDetail : DomainEntity
{
    public Guid OrderId { get; init; }
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    [NotMapped]
    public string ProductName { get; set; } = default!;


    public static OrderDetail CreateForOrder(Guid productId, int quantity)
    {
        return new OrderDetail
        {
            ProductId = productId,
            Quantity = quantity,            
        };
    }
}
