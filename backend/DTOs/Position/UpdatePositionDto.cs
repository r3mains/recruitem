namespace backend.DTOs.Position
{
  public class UpdatePositionDto
  {
    public string? Title { get; set; }
    public int? NumberOfInterviews { get; set; }
    public Guid? StatusId { get; set; }
    public Guid? ReviewerId { get; set; }
    public string? ClosedReason { get; set; }
    public List<PositionSkillDto>? Skills { get; set; }
  }
}
