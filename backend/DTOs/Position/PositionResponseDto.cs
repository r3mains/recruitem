namespace backend.DTOs.Position
{
  public class PositionResponseDto
  {
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int NumberOfInterviews { get; set; }
    public string? ClosedReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public PositionStatusDto Status { get; set; } = null!;
    public PositionReviewerDto? Reviewer { get; set; }
    public List<PositionSkillResponseDto> Skills { get; set; } = new List<PositionSkillResponseDto>();

    public int JobCount { get; set; }
  }

  public class PositionStatusDto
  {
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty;
  }

  public class PositionReviewerDto
  {
    public Guid Id { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
  }

  public class PositionSkillResponseDto
  {
    public Guid Id { get; set; }
    public string SkillName { get; set; } = string.Empty;
  }

  public class PositionListDto
  {
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? ReviewerName { get; set; }
    public int NumberOfInterviews { get; set; }
    public DateTime CreatedAt { get; set; }
    public int JobCount { get; set; }
    public int SkillCount { get; set; }
  }
}
