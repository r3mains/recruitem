namespace backend.DTOs.EmailTemplate;

public record CreateEmailTemplateDto(
  string Name,
  string Subject,
  string Body,
  string? Description,
  string Category,
  string? AvailableVariables,
  bool IsActive
);

public record UpdateEmailTemplateDto(
  string? Name,
  string? Subject,
  string? Body,
  string? Description,
  string? Category,
  string? AvailableVariables,
  bool? IsActive
);

public record EmailTemplateDto(
  Guid Id,
  string Name,
  string Subject,
  string Body,
  string? Description,
  string Category,
  string? AvailableVariables,
  bool IsActive,
  DateTime CreatedAt,
  DateTime? UpdatedAt
);

public record ApplyTemplateDto(
  Guid TemplateId,
  string ToEmail,
  string? ToName,
  Dictionary<string, string> Variables
);

public record PreviewTemplateDto(
  Guid TemplateId,
  Dictionary<string, string> Variables
);

public record TemplatePreviewResultDto(
  string Subject,
  string Body
);
