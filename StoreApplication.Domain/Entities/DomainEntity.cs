namespace StoreApplication.Domain.Entities;

public class DomainEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime LastModifiedOn { get; set; }
}