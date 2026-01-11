using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
  public class CandidateQualification
  {
    public Guid Id { get; set; }

    public Guid CandidateId { get; set; }
    public Candidate Candidate { get; set; } = null!;

    public Guid QualificationId { get; set; }
    public Qualification Qualification { get; set; } = null!;

    public DateTime? CompletedOn { get; set; }

    public decimal? Grade { get; set; }
  }
}
