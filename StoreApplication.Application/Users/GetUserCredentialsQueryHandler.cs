using MediatR;
using StoreApplication.Application.Dtos;
using StoreApplication.Domain.Ports;

namespace StoreApplication.Application.Users;

public class GetUserCredentialsQueryHandler : IRequestHandler<GetUserCredentialsQuery, UserDto>
{
    private readonly ICredentialsRepository _credentialsRepository;
    private readonly ILogMessageService _resourceManager;

    public GetUserCredentialsQueryHandler(ICredentialsRepository credentialsRepository, ILogMessageService resourceManager)
    {
        _credentialsRepository = credentialsRepository ?? throw new ArgumentNullException(nameof(credentialsRepository));
        _resourceManager = resourceManager;
    }

    public async Task<UserDto> Handle(GetUserCredentialsQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        var user = await _credentialsRepository.GetCredentialsAsync(request.Username, request.Password, request.Rol);
        if (user == null || user.Role != _resourceManager.User)
        {
            throw new UnauthorizedAccessException("User credentials are not valid or access not allowed.");
        }

        return new UserDto
        {
            Id = user.Id,
            Username = user.UserName,
            Email = user.Email            
        };
    }
}
