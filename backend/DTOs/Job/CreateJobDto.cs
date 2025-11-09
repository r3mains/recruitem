namespace backend.DTOs.Job
{
  public class CreateJobDto
  {
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid JobTypeId { get; set; }
    public Guid AddressId { get; set; }
    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }
    public Guid PositionId { get; set; }
    public List<Guid> RequiredSkillIds { get; set; } = new List<Guid>();
    public List<Guid> PreferredSkillIds { get; set; } = new List<Guid>();
    public List<JobQualificationDto> Qualifications { get; set; } = new List<JobQualificationDto>();
  }

  public class JobQualificationDto
  {
    public Guid QualificationId { get; set; }
    public double? MinRequired { get; set; }
  }
}
