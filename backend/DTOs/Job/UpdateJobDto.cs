namespace backend.DTOs.Job
{
  public class UpdateJobDto
  {
    public string? Title { get; set; }
    public string? Description { get; set; }
    public Guid? JobTypeId { get; set; }
    public Guid? AddressId { get; set; }
    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }
    public Guid? StatusId { get; set; }
    public string? ClosedReason { get; set; }
    public List<Guid>? RequiredSkillIds { get; set; }
    public List<Guid>? PreferredSkillIds { get; set; }
    public List<JobQualificationDto>? Qualifications { get; set; }
  }
}
