namespace StoreApplication.Api.Dtos;
public record LoginRequestDto(
    string Username,
    string Password,
    string Rol
);