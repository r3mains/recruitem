namespace backend.DTOs.Document;

public record UploadDocumentDto(
  Guid CandidateId,
  Guid DocumentTypeId,
  IFormFile File
);

public record UpdateDocumentDto(
  Guid DocumentTypeId,
  string? OriginalFileName
);

public record DocumentDto(
  Guid Id,
  Guid CandidateId,
  string CandidateName,
  Guid DocumentTypeId,
  string DocumentType,
  string Url,
  string? OriginalFileName,
  string? MimeType,
  long? SizeBytes,
  DateTime? UploadedAt,
  Guid? UploadedBy,
  string? UploadedByName
);

public record DocumentListDto(
  Guid Id,
  Guid CandidateId,
  string CandidateName,
  string DocumentType,
  string? OriginalFileName,
  long? SizeBytes,
  DateTime? UploadedAt
);

public record DocumentTypeDto(
  Guid Id,
  string Type,
  int DocumentCount
);

public record CreateDocumentTypeDto(
  string Type
);

public record UpdateDocumentTypeDto(
  string Type
);
