namespace backend.DTOs.Verification;

public record CreateVerificationDto(
  Guid CandidateId,
  Guid DocumentId,
  Guid StatusId,
  string? Comments,
  Guid VerifiedBy
);

public record UpdateVerificationDto(
  Guid StatusId,
  string? Comments
);

public record VerificationDto(
  Guid Id,
  Guid CandidateId,
  string CandidateName,
  Guid DocumentId,
  string DocumentUrl,
  string DocumentType,
  Guid StatusId,
  string Status,
  string? Comments,
  Guid? VerifiedBy,
  string? VerifiedByName,
  DateTime? VerifiedAt
);

public record VerificationListDto(
  Guid Id,
  string CandidateName,
  string DocumentType,
  string Status,
  string VerifiedByName,
  DateTime? VerifiedAt
);
