namespace Backend.Dtos.Candidates;

public class CandidateSearchDto
{
  public string? Name { get; set; }
  public string? Email { get; set; }
  public List<Guid>? SkillIds { get; set; }
  public int? MinExperience { get; set; }
  public int? MaxExperience { get; set; }
  public Guid? CityId { get; set; }
  public Guid? StateId { get; set; }
  public int Page { get; set; } = 1;
  public int PageSize { get; set; } = 10;
}
