namespace Backend.Dtos.Positions;

public class PositionDto
{
  public Guid Id { get; set; }
  public string Title { get; set; } = string.Empty;
  public Guid StatusId { get; set; }
  public string? ClosedReason { get; set; }
  public int NumberOfInterviews { get; set; }
  public Guid? ReviewerId { get; set; }
}

public class PositionCreateDto
{
  public string Title { get; set; } = string.Empty;
  public Guid StatusId { get; set; }
  public string? ClosedReason { get; set; }
  public int NumberOfInterviews { get; set; }
  public Guid? ReviewerId { get; set; }
}

public class PositionUpdateDto
{
  public string? Title { get; set; }
  public Guid? StatusId { get; set; }
  public string? ClosedReason { get; set; }
  public int? NumberOfInterviews { get; set; }
  public Guid? ReviewerId { get; set; }
}
