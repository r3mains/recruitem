using FluentValidation;
using recruitem_backend.Controllers;

namespace recruitem_backend.Validators
{
    public class CreateJobRequestValidator : AbstractValidator<CreateJobRequest>
    {
        public CreateJobRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required");

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Location is required")
                .MaximumLength(100).WithMessage("Location cannot exceed 100 characters");

            RuleFor(x => x.SalaryMin)
                .GreaterThan(0).WithMessage("Minimum salary must be greater than 0")
                .When(x => x.SalaryMin.HasValue);

            RuleFor(x => x.SalaryMax)
                .GreaterThan(0).WithMessage("Maximum salary must be greater than 0")
                .When(x => x.SalaryMax.HasValue);

            RuleFor(x => x.SalaryMax)
                .GreaterThanOrEqualTo(x => x.SalaryMin).WithMessage("Maximum salary must be greater than or equal to minimum salary")
                .When(x => x.SalaryMin.HasValue && x.SalaryMax.HasValue);
        }
    }
}
