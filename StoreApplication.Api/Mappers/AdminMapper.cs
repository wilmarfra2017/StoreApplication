using StoreApplication.Api.Dtos;
using StoreApplication.Application.Admins;

namespace StoreApplication.Api.Mappers;

public static class AdminMapper
{
    public static GetAdminCredentialsQuery ToGetAdminCredentialsQuery(this LoginRequestDto loginRequestDto)
    {
        ArgumentNullException.ThrowIfNull(loginRequestDto, nameof(loginRequestDto));

        return new GetAdminCredentialsQuery(
            loginRequestDto.Username,
            loginRequestDto.Password,
            loginRequestDto.Rol
        );
    }
}
