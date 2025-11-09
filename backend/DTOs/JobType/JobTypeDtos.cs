namespace backend.DTOs.JobType
{
  public class CreateJobTypeDto
  {
    public string Type { get; set; } = string.Empty;
  }

  public class UpdateJobTypeDto
  {
    public string Type { get; set; } = string.Empty;
  }

  public class JobTypeResponseDto
  {
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;

    public int JobCount { get; set; }
  }

  public class JobTypeListDto
  {
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public int JobCount { get; set; }
  }
}
