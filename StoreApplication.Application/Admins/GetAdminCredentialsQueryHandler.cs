using MediatR;
using Microsoft.Extensions.Logging;
using StoreApplication.Application.Dtos;
using StoreApplication.Domain.Ports;

namespace StoreApplication.Application.Admins;

public class GetAdminCredentialsQueryHandler : IRequestHandler<GetAdminCredentialsQuery, AdminDto>
{
    private readonly ICredentialsRepository _credentialsRepository;
    private readonly ILogMessageService _resourceManager;
    private readonly ILogger<GetAdminCredentialsQueryHandler> _logger;

    public GetAdminCredentialsQueryHandler(ICredentialsRepository credentialsRepository, ILogMessageService resourceManager,
        ILogger<GetAdminCredentialsQueryHandler> logger)
    {
        _credentialsRepository = credentialsRepository ?? throw new ArgumentNullException(nameof(credentialsRepository));
        _resourceManager = resourceManager;
        _logger = logger;
    }

    public async Task<AdminDto> Handle(GetAdminCredentialsQuery request, CancellationToken cancellationToken)
    {
        _logger.Log(LogLevel.Information, "Method Handle - GetAdminCredentialsQueryHandler");

        ArgumentNullException.ThrowIfNull(request, nameof(request));

        var admin = await _credentialsRepository.GetCredentialsAsync(request.Username, request.Password, request.Rol);
        if (admin is null)
        {
            throw new UnauthorizedAccessException(_resourceManager.AdminCredentialsNotValid);            
        }

        return new AdminDto
        {
            Id = admin.Id,
            Username = admin.UserName,
            Email = admin.Email
        };
    }
}
