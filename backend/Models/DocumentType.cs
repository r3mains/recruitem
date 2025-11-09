namespace backend.Models;

public class DocumentType
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public string Type { get; set; } = string.Empty;

  // Navigation properties
  public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
}
