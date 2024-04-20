using FluentValidation;
using StoreApplication.Api.Dtos;

namespace StoreApplication.Api.Validators
{
    public class LoginDtoValidator : AbstractValidator<LoginRequestDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("UserName must not be empty");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password must not be empty");
        }
    }
}
