using backend.DTOs.Document;
using FluentValidation;

namespace backend.Validators.DocumentValidators;

public class UploadDocumentValidator : AbstractValidator<UploadDocumentDto>
{
  private static readonly string[] AllowedExtensions = { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png", ".txt", ".zip" };
  private const long MaxFileSize = 10 * 1024 * 1024; // 10 MB

  public UploadDocumentValidator()
  {
    RuleFor(x => x.CandidateId)
      .NotEmpty().WithMessage("Candidate ID is required");

    RuleFor(x => x.DocumentTypeId)
      .NotEmpty().WithMessage("Document type is required");

    RuleFor(x => x.File)
      .NotNull().WithMessage("File is required")
      .Must(file => file != null && file.Length > 0).WithMessage("File cannot be empty")
      .Must(file => file == null || file.Length <= MaxFileSize).WithMessage($"File size must not exceed {MaxFileSize / 1024 / 1024} MB")
      .Must(file => file == null || AllowedExtensions.Contains(Path.GetExtension(file.FileName).ToLowerInvariant()))
        .WithMessage($"File type not allowed. Allowed types: {string.Join(", ", AllowedExtensions)}");
  }
}

public class UpdateDocumentValidator : AbstractValidator<UpdateDocumentDto>
{
  public UpdateDocumentValidator()
  {
    RuleFor(x => x.DocumentTypeId)
      .NotEmpty().WithMessage("Document type is required");
  }
}

public class CreateDocumentTypeValidator : AbstractValidator<CreateDocumentTypeDto>
{
  public CreateDocumentTypeValidator()
  {
    RuleFor(x => x.Type)
      .NotEmpty().WithMessage("Document type is required")
      .MaximumLength(100).WithMessage("Document type must not exceed 100 characters");
  }
}

public class UpdateDocumentTypeValidator : AbstractValidator<UpdateDocumentTypeDto>
{
  public UpdateDocumentTypeValidator()
  {
    RuleFor(x => x.Type)
      .NotEmpty().WithMessage("Document type is required")
      .MaximumLength(100).WithMessage("Document type must not exceed 100 characters");
  }
}
