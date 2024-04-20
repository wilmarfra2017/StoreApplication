using Microsoft.EntityFrameworkCore;
using StoreApplication.Domain.Entities;
using StoreApplication.Domain.Ports;
using StoreApplication.Infrastructure.DataSource;

namespace StoreApplication.Infrastructure.Adapters;

[Repository]
public class CredentialsRepository : ICredentialsRepository
{
    private readonly DataContext _context;

    public CredentialsRepository(DataContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<User> GetCredentialsAsync(string username, string password, string role)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Username and password cannot be empty.");
        }

        var user = await _context.Users
        .AsNoTracking()
        .FirstOrDefaultAsync(u => u.UserName == username && u.PasswordHash == password && u.Role == role);

        if (user == null)
        {
            throw new InvalidOperationException("No user found with the specified credentials.");
        }

        return user;
    }
}
