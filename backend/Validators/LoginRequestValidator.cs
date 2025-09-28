using FluentValidation;
using recruitem_backend.Controllers;

namespace recruitem_backend.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Please enter your email")
                .EmailAddress().WithMessage("Please enter a valid email");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Please enter your password");
        }
    }
}
