using iTextSharp.text;
using iTextSharp.text.pdf;

namespace backend.Services.IServices;

public interface IPdfGenerationService
{
  Task<string> GenerateOfferLetterPdfAsync(
    string candidateName,
    string jobTitle,
    string companyName,
    string? companyAddress,
    decimal salary,
    DateTime joiningDate,
    DateTime offerDate,
    string? benefits,
    string? additionalTerms,
    string? signatoryName,
    string? signatoryDesignation
  );
  
  Task<byte[]> GetPdfBytesAsync(string filePath);
}
