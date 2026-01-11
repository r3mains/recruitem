using backend.DTOs.Resume;

namespace backend.Services.IServices;

public interface IResumeParserService
{
  Task<ParsedResumeDto> ParseResumeAsync(IFormFile file);
  Task<string> ExtractTextFromPdfAsync(Stream pdfStream);
  ParsedResumeDto ExtractInformationFromText(string text);
}
