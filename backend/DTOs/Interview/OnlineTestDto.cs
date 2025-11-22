using System.ComponentModel.DataAnnotations;

namespace backend.DTOs.Interview
{
  public class CreateOnlineTestDto
  {
    [Required]
    public Guid JobApplicationId { get; set; }
  }

  public class UpdateOnlineTestDto
  {
    [Range(0, 100)]
    public decimal? Score { get; set; }

    public string? Result { get; set; }
  }

  public class OnlineTestDto
  {
    public Guid Id { get; set; }
    public Guid JobApplicationId { get; set; }
    public string? CandidateName { get; set; }
    public string? JobTitle { get; set; }
    public decimal? Score { get; set; }
    public string? Result { get; set; }
    public DateTime? TakenAt { get; set; }
  }

  public class InterviewTypeDto
  {
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
  }
}
