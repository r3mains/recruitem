namespace backend.DTOs.Screening;

public class ScreenResumeDto
{
  public Guid JobApplicationId { get; set; }
  public decimal? Score { get; set; }
  public string? Comments { get; set; }
  public bool Approved { get; set; } = false;
}

public class AddCommentDto
{
  public Guid JobApplicationId { get; set; }
  public string Comment { get; set; } = string.Empty;
}

public class UpdateCandidateSkillsDto
{
  public List<CandidateSkillUpdateDto> Skills { get; set; } = new();
}

public class CandidateSkillUpdateDto
{
  public Guid SkillId { get; set; }
  public int? YearsOfExperience { get; set; }
  public string? ProficiencyLevel { get; set; }
}

public class AssignReviewerDto
{
  public Guid PositionId { get; set; }
  public Guid ReviewerId { get; set; }
}

public class ShortlistCandidateDto
{
  public Guid JobApplicationId { get; set; }
  public string? Comments { get; set; }
}

public class ScreeningResponseDto
{
  public Guid Id { get; set; }
  public Guid JobApplicationId { get; set; }
  public string CandidateName { get; set; } = string.Empty;
  public string JobTitle { get; set; } = string.Empty;
  public decimal? Score { get; set; }
  public string? Comments { get; set; }
  public string Status { get; set; } = string.Empty;
  public string ScreenedBy { get; set; } = string.Empty;
  public DateTime ScreenedAt { get; set; }
}

public class CandidateSkillScreeningDto
{
  public Guid Id { get; set; }
  public string SkillName { get; set; } = string.Empty;
  public int? YearsOfExperience { get; set; }
  public string? ProficiencyLevel { get; set; }
  public bool IsVerified { get; set; }
}

public class CommentResponseDto
{
  public Guid Id { get; set; }
  public string Comment { get; set; } = string.Empty;
  public string CommenterName { get; set; } = string.Empty;
  public DateTime CreatedAt { get; set; }
}

public class ShortlistResponseDto
{
  public Guid JobApplicationId { get; set; }
  public string CandidateName { get; set; } = string.Empty;
  public string JobTitle { get; set; } = string.Empty;
  public string Status { get; set; } = string.Empty;
  public string? Comments { get; set; }
  public DateTime ShortlistedAt { get; set; }
  public string ShortlistedBy { get; set; } = string.Empty;
}
