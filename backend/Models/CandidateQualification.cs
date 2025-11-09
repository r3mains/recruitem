using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
  public class CandidateQualification
  {
    public Guid Id { get; set; }

    [Required]
    public Guid CandidateId { get; set; }
    public Candidate Candidate { get; set; } = null!;

    [Required]
    public Guid QualificationId { get; set; }
    public Qualification Qualification { get; set; } = null!;

    [Column(TypeName = "date")]
    public DateTime? CompletedOn { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal? Grade { get; set; }
  }
}
