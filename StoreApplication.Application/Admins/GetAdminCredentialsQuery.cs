using MediatR;
using StoreApplication.Application.Dtos;

namespace StoreApplication.Application.Admins;

public class GetAdminCredentialsQuery : IRequest<AdminDto>
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Rol { get; set; }

    public GetAdminCredentialsQuery(string username, string password, string rol)
    {
        Username = username;
        Password = password;
        Rol = rol;
    }
}
