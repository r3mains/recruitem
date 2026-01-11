using backend.Data;
using backend.DTOs.Document;
using backend.Models;
using backend.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories;

public class DocumentRepository(ApplicationDbContext context) : IDocumentRepository
{
  private readonly ApplicationDbContext _context = context;

  public async Task<IEnumerable<DocumentListDto>> GetAllDocumentsAsync()
  {
    return await _context.Documents
      .Include(d => d.Candidate)
        .ThenInclude(c => c.User)
      .Include(d => d.DocumentType)
      .Select(d => new DocumentListDto(
        d.Id,
        d.CandidateId,
        d.Candidate.FullName ?? "",
        d.DocumentType.Type,
        d.OriginalFileName,
        d.SizeBytes,
        d.UploadedAt
      ))
      .ToListAsync();
  }

  public async Task<IEnumerable<DocumentListDto>> GetDocumentsByCandidateAsync(Guid candidateId)
  {
    return await _context.Documents
      .Include(d => d.Candidate)
        .ThenInclude(c => c.User)
      .Include(d => d.DocumentType)
      .Where(d => d.CandidateId == candidateId)
      .Select(d => new DocumentListDto(
        d.Id,
        d.CandidateId,
        d.Candidate.FullName ?? "",
        d.DocumentType.Type,
        d.OriginalFileName,
        d.SizeBytes,
        d.UploadedAt
      ))
      .ToListAsync();
  }

  public async Task<DocumentDto?> GetDocumentByIdAsync(Guid id)
  {
    return await _context.Documents
      .Include(d => d.Candidate)
        .ThenInclude(c => c.User)
      .Include(d => d.DocumentType)
      .Include(d => d.UploadedByUser)
      .Where(d => d.Id == id)
      .Select(d => new DocumentDto(
        d.Id,
        d.CandidateId,
        d.Candidate.FullName ?? "",
        d.DocumentTypeId,
        d.DocumentType.Type,
        d.Url,
        d.OriginalFileName,
        d.MimeType,
        d.SizeBytes,
        d.UploadedAt,
        d.UploadedBy,
        d.UploadedByUser != null ? d.UploadedByUser.UserName : null
      ))
      .FirstOrDefaultAsync();
  }

  public async Task<DocumentDto> CreateDocumentAsync(Guid candidateId, Guid documentTypeId, string url, string? originalFileName, string? mimeType, long? sizeBytes, Guid? uploadedBy)
  {
    var document = new Document
    {
      Id = Guid.NewGuid(),
      CandidateId = candidateId,
      DocumentTypeId = documentTypeId,
      Url = url,
      OriginalFileName = originalFileName,
      MimeType = mimeType,
      SizeBytes = sizeBytes,
      UploadedAt = DateTime.UtcNow,
      UploadedBy = uploadedBy
    };

    _context.Documents.Add(document);
    await _context.SaveChangesAsync();

    return (await GetDocumentByIdAsync(document.Id))!;
  }

  public async Task<DocumentDto?> UpdateDocumentAsync(Guid id, UpdateDocumentDto dto)
  {
    var document = await _context.Documents.FindAsync(id);
    if (document == null)
      return null;

    document.DocumentTypeId = dto.DocumentTypeId;
    if (dto.OriginalFileName != null)
      document.OriginalFileName = dto.OriginalFileName;

    await _context.SaveChangesAsync();
    return await GetDocumentByIdAsync(id);
  }

  public async Task<bool> DeleteDocumentAsync(Guid id)
  {
    var document = await _context.Documents.FindAsync(id);
    if (document == null)
      return false;

    _context.Documents.Remove(document);
    await _context.SaveChangesAsync();
    return true;
  }

  // Document Types
  public async Task<IEnumerable<DocumentTypeDto>> GetAllDocumentTypesAsync()
  {
    return await _context.DocumentTypes
      .Select(dt => new DocumentTypeDto(
        dt.Id,
        dt.Type,
        dt.Documents.Count
      ))
      .ToListAsync();
  }

  public async Task<DocumentTypeDto?> GetDocumentTypeByIdAsync(Guid id)
  {
    return await _context.DocumentTypes
      .Where(dt => dt.Id == id)
      .Select(dt => new DocumentTypeDto(
        dt.Id,
        dt.Type,
        dt.Documents.Count
      ))
      .FirstOrDefaultAsync();
  }

  public async Task<DocumentTypeDto> CreateDocumentTypeAsync(CreateDocumentTypeDto dto)
  {
    var documentType = new DocumentType
    {
      Id = Guid.NewGuid(),
      Type = dto.Type
    };

    _context.DocumentTypes.Add(documentType);
    await _context.SaveChangesAsync();

    return (await GetDocumentTypeByIdAsync(documentType.Id))!;
  }

  public async Task<DocumentTypeDto?> UpdateDocumentTypeAsync(Guid id, UpdateDocumentTypeDto dto)
  {
    var documentType = await _context.DocumentTypes.FindAsync(id);
    if (documentType == null)
      return null;

    documentType.Type = dto.Type;
    await _context.SaveChangesAsync();

    return await GetDocumentTypeByIdAsync(id);
  }

  public async Task<bool> DeleteDocumentTypeAsync(Guid id)
  {
    var documentType = await _context.DocumentTypes.FindAsync(id);
    if (documentType == null)
      return false;

    _context.DocumentTypes.Remove(documentType);
    await _context.SaveChangesAsync();
    return true;
  }
}
