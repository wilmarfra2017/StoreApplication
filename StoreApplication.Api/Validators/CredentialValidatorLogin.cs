using FluentValidation;
using StoreApplication.Application.Admins;

namespace StoreApplication.Api.Validators
{
    public class CredentialValidatorLogin : AbstractValidator<GetAdminCredentialsQuery>
    {
        public CredentialValidatorLogin()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("UserName must not be empty.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password must not be empty.");
        }
    }
}
