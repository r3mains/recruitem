using FluentValidation;
using backend.DTOs.EmailTemplate;

namespace backend.Validators;

public class CreateEmailTemplateValidator : AbstractValidator<CreateEmailTemplateDto>
{
  public CreateEmailTemplateValidator()
  {
    RuleFor(x => x.Name)
      .NotEmpty().WithMessage("Template name is required")
      .MaximumLength(200).WithMessage("Template name cannot exceed 200 characters");

    RuleFor(x => x.Subject)
      .NotEmpty().WithMessage("Subject is required")
      .MaximumLength(500).WithMessage("Subject cannot exceed 500 characters");

    RuleFor(x => x.Body)
      .NotEmpty().WithMessage("Body is required");

    RuleFor(x => x.Category)
      .NotEmpty().WithMessage("Category is required")
      .MaximumLength(100).WithMessage("Category cannot exceed 100 characters");

    RuleFor(x => x.Description)
      .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");
  }
}

public class UpdateEmailTemplateValidator : AbstractValidator<UpdateEmailTemplateDto>
{
  public UpdateEmailTemplateValidator()
  {
    RuleFor(x => x.Name)
      .MaximumLength(200).WithMessage("Template name cannot exceed 200 characters")
      .When(x => x.Name != null);

    RuleFor(x => x.Subject)
      .MaximumLength(500).WithMessage("Subject cannot exceed 500 characters")
      .When(x => x.Subject != null);

    RuleFor(x => x.Category)
      .MaximumLength(100).WithMessage("Category cannot exceed 100 characters")
      .When(x => x.Category != null);

    RuleFor(x => x.Description)
      .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters")
      .When(x => x.Description != null);
  }
}

public class ApplyTemplateValidator : AbstractValidator<ApplyTemplateDto>
{
  public ApplyTemplateValidator()
  {
    RuleFor(x => x.TemplateId)
      .NotEmpty().WithMessage("Template ID is required");

    RuleFor(x => x.ToEmail)
      .NotEmpty().WithMessage("Recipient email is required")
      .EmailAddress().WithMessage("Invalid email address");

    RuleFor(x => x.Variables)
      .NotNull().WithMessage("Variables dictionary is required");
  }
}

public class PreviewTemplateValidator : AbstractValidator<PreviewTemplateDto>
{
  public PreviewTemplateValidator()
  {
    RuleFor(x => x.TemplateId)
      .NotEmpty().WithMessage("Template ID is required");

    RuleFor(x => x.Variables)
      .NotNull().WithMessage("Variables dictionary is required");
  }
}
