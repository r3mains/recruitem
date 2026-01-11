namespace backend.Models;

public class OfferLetter
{
  public Guid Id { get; set; }
  public Guid JobApplicationId { get; set; }
  public JobApplication? JobApplication { get; set; }
  public DateTime OfferDate { get; set; }
  public DateTime? JoiningDate { get; set; }
  public decimal Salary { get; set; }
  public string? Benefits { get; set; }
  public string? AdditionalTerms { get; set; }
  public string Status { get; set; } = string.Empty;
  public DateTime? AcceptedDate { get; set; }
  public DateTime? RejectedDate { get; set; }
  public string? RejectionReason { get; set; }
  public DateTime? ExpiryDate { get; set; }
  public string? GeneratedPdfPath { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }
  public Guid CreatedBy { get; set; }
  public User? Creator { get; set; }
}
