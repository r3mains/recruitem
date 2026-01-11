using backend.DTOs.Candidate;

namespace backend.Services.IServices;

public interface ICandidateBulkImportService
{
  Task<BulkImportResultDto> ImportCandidatesFromExcelAsync(Stream fileStream, string fileName);
}
