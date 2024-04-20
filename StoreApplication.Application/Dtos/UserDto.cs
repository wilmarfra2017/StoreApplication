namespace StoreApplication.Application.Dtos;

public class UserDto
{
    public Guid Id { get; init; }
    public string? Username { get; init; }
    public string? Email { get; init; }
}
