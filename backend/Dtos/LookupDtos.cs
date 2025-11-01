namespace Backend.Dtos;

public class SkillDto
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
}

public class JobTypeDto
{
  public Guid Id { get; set; }
  public string Type { get; set; } = string.Empty;
}

public class StatusTypeDto
{
  public Guid Id { get; set; }
  public string Context { get; set; } = string.Empty;
  public string Status { get; set; } = string.Empty;
}

public class QualificationDto
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
}

public class CityDto
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public Guid StateId { get; set; }
  public string? StateName { get; set; }
}

public class StateDto
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public Guid CountryId { get; set; }
  public string? CountryName { get; set; }
}

public class CountryDto
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
}
