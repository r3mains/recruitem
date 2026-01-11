namespace backend.Models;

public class Job
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public Guid RecruiterId { get; set; }
  public string Title { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public Guid JobTypeId { get; set; }
  public Guid AddressId { get; set; }
  public decimal? SalaryMin { get; set; }
  public decimal? SalaryMax { get; set; }
  public Guid PositionId { get; set; }
  public Guid StatusId { get; set; }
  public string? ClosedReason { get; set; }
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
  public bool IsDeleted { get; set; } = false;


  public virtual Employee Recruiter { get; set; } = null!;
  public virtual JobType JobType { get; set; } = null!;
  public virtual Address Address { get; set; } = null!;
  public virtual Position Position { get; set; } = null!;
  public virtual JobStatus Status { get; set; } = null!;
  public virtual ICollection<JobSkill> JobSkills { get; set; } = [];
  public virtual ICollection<JobQualification> JobQualifications { get; set; } = [];
  public ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();
}
