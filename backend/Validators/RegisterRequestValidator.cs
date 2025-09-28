using FluentValidation;
using recruitem_backend.Controllers;

namespace recruitem_backend.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Please enter your email")
                .EmailAddress().WithMessage("Please enter a valid email address");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Please enter a password")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters");

            RuleFor(x => x.RoleName)
                .NotEmpty().WithMessage("Please select a role")
                .Must(BeValidRole).WithMessage("Please select a valid role");
        }

        private bool BeValidRole(string roleName)
        {
            var validRoles = new string[] { "Recruiter", "HR", "Interviewer", "Reviewer", "Admin", "Candidate", "Viewer" };
            return validRoles.Contains(roleName);
        }
    }
}
