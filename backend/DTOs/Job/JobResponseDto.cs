namespace backend.DTOs.Job
{
  public class JobResponseDto
  {
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }
    public string? ClosedReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public JobRecruiterDto Recruiter { get; set; } = null!;
    public JobTypeDto JobType { get; set; } = null!;
    public JobAddressDto Address { get; set; } = null!;
    public JobPositionDto Position { get; set; } = null!;
    public JobStatusDto Status { get; set; } = null!;

    public List<JobSkillDto> Skills { get; set; } = new List<JobSkillDto>();
    public List<JobQualificationResponseDto> Qualifications { get; set; } = new List<JobQualificationResponseDto>();

    public int? ApplicationCount { get; set; }
  }

  public class JobRecruiterDto
  {
    public Guid Id { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
  }

  public class JobTypeDto
  {
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
  }

  public class JobAddressDto
  {
    public Guid Id { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? Locality { get; set; }
    public string? Pincode { get; set; }
    public string? CityName { get; set; }
    public string? StateName { get; set; }
    public string? CountryName { get; set; }
  }

  public class JobPositionDto
  {
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
  }

  public class JobStatusDto
  {
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty;
  }

  public class JobSkillDto
  {
    public Guid Id { get; set; }
    public string SkillName { get; set; } = string.Empty;
    public bool Required { get; set; }
  }

  public class JobQualificationResponseDto
  {
    public Guid Id { get; set; }
    public string QualificationName { get; set; } = string.Empty;
    public double? MinRequired { get; set; }
  }

  public class JobListDto
  {
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string JobType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string RecruiterName { get; set; } = string.Empty;
    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }
    public DateTime CreatedAt { get; set; }
    public int ApplicationCount { get; set; }
  }
}
