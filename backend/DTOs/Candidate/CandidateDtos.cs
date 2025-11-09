namespace backend.DTOs.Candidate
{
  public class CreateCandidateDto
  {
    public string FullName { get; set; } = string.Empty;
    public string? ContactNumber { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public CreateAddressDto? Address { get; set; }

    public List<CreateCandidateSkillDto>? Skills { get; set; }
    public List<CreateCandidateQualificationDto>? Qualifications { get; set; }
  }

  public class UpdateCandidateDto
  {
    public string FullName { get; set; } = string.Empty;
    public string? ContactNumber { get; set; }

    public CreateAddressDto? Address { get; set; }

    public List<CreateCandidateSkillDto>? Skills { get; set; }
    public List<CreateCandidateQualificationDto>? Qualifications { get; set; }
  }

  public class CandidateResponseDto
  {
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? FullName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? ContactNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public AddressDto? Address { get; set; }

    public List<CandidateSkillDto> Skills { get; set; } = new();
    public List<CandidateQualificationDto> Qualifications { get; set; } = new();

    public int ApplicationCount { get; set; }
    public int DocumentCount { get; set; }
  }

  public class CandidateListDto
  {
    public Guid Id { get; set; }
    public string? FullName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? ContactNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Location { get; set; }
    public int ApplicationCount { get; set; }
    public int SkillCount { get; set; }
  }

  public class CreateCandidateSkillDto
  {
    public Guid SkillId { get; set; }
    public int? YearOfExperience { get; set; }
  }

  public class CandidateSkillDto
  {
    public Guid Id { get; set; }
    public Guid SkillId { get; set; }
    public string SkillName { get; set; } = string.Empty;
    public int? YearOfExperience { get; set; }
  }

  public class CreateCandidateQualificationDto
  {
    public Guid QualificationId { get; set; }
    public DateTime? CompletedOn { get; set; }
    public decimal? Grade { get; set; }
  }

  public class CandidateQualificationDto
  {
    public Guid Id { get; set; }
    public Guid QualificationId { get; set; }
    public string QualificationName { get; set; } = string.Empty;
    public DateTime? CompletedOn { get; set; }
    public decimal? Grade { get; set; }
  }

  public class CreateAddressDto
  {
    public string? Street { get; set; }
    public Guid CityId { get; set; }
    public string? PostalCode { get; set; }
  }

  public class AddressDto
  {
    public Guid Id { get; set; }
    public string? Street { get; set; }
    public string? PostalCode { get; set; }
    public string CityName { get; set; } = string.Empty;
    public string StateName { get; set; } = string.Empty;
  }
}
