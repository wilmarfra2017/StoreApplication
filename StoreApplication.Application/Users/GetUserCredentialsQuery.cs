using MediatR;
using StoreApplication.Application.Dtos;

namespace StoreApplication.Application.Users;

public class GetUserCredentialsQuery : IRequest<UserDto>
{
    public string Username { get; }
    public string Password { get; }
    public string Rol { get; }

    public GetUserCredentialsQuery(string username, string password, string rol)
    {
        Username = username;
        Password = password;
        Rol = rol;
    }
}
