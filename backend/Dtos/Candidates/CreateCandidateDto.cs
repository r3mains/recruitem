namespace Backend.Dtos.Candidates;

public class CreateCandidateDto
{
  public string Email { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
  public string? FullName { get; set; }
  public string? ContactNumber { get; set; }
  public string? ResumeUrl { get; set; }
  public CreateAddressDto? Address { get; set; }
  public List<CreateCandidateSkillDto> Skills { get; set; } = [];
  public List<CreateCandidateQualificationDto> Qualifications { get; set; } = [];
}

public class CreateCandidateSkillDto
{
  public Guid SkillId { get; set; }
  public int YearsOfExperience { get; set; }
}

public class CreateCandidateQualificationDto
{
  public Guid QualificationId { get; set; }
  public DateTime? YearCompleted { get; set; }
  public double? Grade { get; set; }
}

public class CreateAddressDto
{
  public string? AddressLine1 { get; set; }
  public string? AddressLine2 { get; set; }
  public string? Locality { get; set; }
  public string? Pincode { get; set; }
  public Guid? CityId { get; set; }
}
