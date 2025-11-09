namespace backend.DTOs.Position
{
  public class CreatePositionDto
  {
    public string Title { get; set; } = string.Empty;
    public int NumberOfInterviews { get; set; } = 1;
    public Guid? ReviewerId { get; set; }
    public List<PositionSkillDto> Skills { get; set; } = new List<PositionSkillDto>();
  }

  public class PositionSkillDto
  {
    public Guid SkillId { get; set; }
  }
}
