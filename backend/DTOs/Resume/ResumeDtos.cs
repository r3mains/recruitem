namespace backend.DTOs.Resume;

public record ParseResumeDto(
  IFormFile ResumeFile
);

public record ParsedResumeDto(
  string? FullName,
  string? Email,
  string? Phone,
  string? RawText,
  List<string> Skills,
  List<string> Education,
  int? YearsOfExperience,
  string? Summary
);

public record SaveParsedResumeDto(
  Guid CandidateId,
  string? FullName,
  string? Email,
  string? Phone,
  List<Guid> SkillIds,
  List<Guid> QualificationIds
);
