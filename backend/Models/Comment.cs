using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
  public class Comment
  {
    public Guid Id { get; set; }

    [Required]
    public Guid JobApplicationId { get; set; }
    public JobApplication JobApplication { get; set; } = null!;

    [Required]
    public Guid CommenterId { get; set; }
    public Employee Commenter { get; set; } = null!;

    [Required]
    public string CommentText { get; set; } = string.Empty;

    public DateTime? CreatedAt { get; set; }
  }
}
