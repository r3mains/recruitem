namespace backend.DTOs.OfferLetter;

public record CreateOfferLetterDto(
  Guid JobApplicationId,
  DateTime JoiningDate,
  decimal Salary,
  string? Benefits,
  string? AdditionalTerms,
  DateTime? ExpiryDate
);

public record UpdateOfferLetterDto(
  DateTime? JoiningDate,
  decimal? Salary,
  string? Benefits,
  string? AdditionalTerms,
  DateTime? ExpiryDate
);

public record OfferLetterDto(
  Guid Id,
  Guid JobApplicationId,
  string CandidateName,
  string CandidateEmail,
  string JobTitle,
  string CompanyName,
  DateTime OfferDate,
  DateTime? JoiningDate,
  decimal Salary,
  string? Benefits,
  string? AdditionalTerms,
  string Status,
  DateTime? AcceptedDate,
  DateTime? RejectedDate,
  string? RejectionReason,
  DateTime? ExpiryDate,
  string? GeneratedPdfPath,
  DateTime CreatedAt,
  DateTime? UpdatedAt,
  Guid CreatedBy
);

public record GenerateOfferLetterDto(
  Guid OfferLetterId,
  string? CompanyName,
  string? CompanyAddress,
  string? SignatoryName,
  string? SignatoryDesignation
);

public record AcceptOfferDto(
  Guid OfferLetterId
);

public record RejectOfferDto(
  Guid OfferLetterId,
  string Reason
);
