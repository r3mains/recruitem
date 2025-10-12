namespace Backend.Models;

public class Job
{
  public Guid Id { get; set; }
  public Guid? RecruiterId { get; set; }
  public string Title { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public Guid JobTypeId { get; set; }
  public Guid LocationId { get; set; }
  public decimal? SalaryMin { get; set; }
  public decimal? SalaryMax { get; set; }
  public Guid PositionId { get; set; }
  public Guid StatusId { get; set; }
  public string? ClosedReason { get; set; }
  public DateTime? CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }

  public Employee? Recruiter { get; set; }
  public JobType? JobType { get; set; }
  public Address? Location { get; set; }
  public Position? Position { get; set; }
  public StatusType? Status { get; set; }
  public ICollection<JobApplication> JobApplications { get; set; } = [];
  public ICollection<JobSkill> JobSkills { get; set; } = [];
}
