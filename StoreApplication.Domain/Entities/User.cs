namespace StoreApplication.Domain.Entities;
public record User(
    Guid Id,
    string UserName,
    string PasswordHash,
    string Role,
    string Email
);
