namespace Backend.Models;

public class Job
{
  public Guid Id { get; set; }
  public Guid RecruiterId { get; set; }
  public string Title { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public Guid JobTypeId { get; set; }
  public Guid Location { get; set; }
  public decimal? SalaryMin { get; set; }
  public decimal? SalaryMax { get; set; }
  public Guid PositionId { get; set; }
  public Guid StatusId { get; set; }
  public string? ClosedReason { get; set; }
  public DateTime? CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }
}
