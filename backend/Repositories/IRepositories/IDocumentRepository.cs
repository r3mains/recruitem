using backend.DTOs.Document;

namespace backend.Repositories.IRepositories;

public interface IDocumentRepository
{
  Task<IEnumerable<DocumentListDto>> GetAllDocumentsAsync();
  Task<IEnumerable<DocumentListDto>> GetDocumentsByCandidateAsync(Guid candidateId);
  Task<DocumentDto?> GetDocumentByIdAsync(Guid id);
  Task<DocumentDto> CreateDocumentAsync(Guid candidateId, Guid documentTypeId, string url, string? originalFileName, string? mimeType, long? sizeBytes, Guid? uploadedBy);
  Task<DocumentDto?> UpdateDocumentAsync(Guid id, UpdateDocumentDto dto);
  Task<bool> DeleteDocumentAsync(Guid id);
  
  // Document Types
  Task<IEnumerable<DocumentTypeDto>> GetAllDocumentTypesAsync();
  Task<DocumentTypeDto?> GetDocumentTypeByIdAsync(Guid id);
  Task<DocumentTypeDto> CreateDocumentTypeAsync(CreateDocumentTypeDto dto);
  Task<DocumentTypeDto?> UpdateDocumentTypeAsync(Guid id, UpdateDocumentTypeDto dto);
  Task<bool> DeleteDocumentTypeAsync(Guid id);
}
