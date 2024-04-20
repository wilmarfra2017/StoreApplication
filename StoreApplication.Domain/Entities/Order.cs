namespace StoreApplication.Domain.Entities;
public class Order : DomainEntity
{    
    public Guid UserId { get; set; }
    public string Email { get; set; } = default!;
    public DateTime OrderDate { get; set; }
    public decimal Total { get; set; }
    public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public Order() : base() { }

    public Order(Guid userId, string email, DateTime orderDate, decimal total) : base()
    {
        UserId = userId;
        Email = email;
        OrderDate = orderDate;
        Total = total;
    }
}
