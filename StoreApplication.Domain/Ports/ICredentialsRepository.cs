using StoreApplication.Domain.Entities;

namespace StoreApplication.Domain.Ports;

public interface ICredentialsRepository
{
    Task<User> GetCredentialsAsync(string username, string password, string role);
}