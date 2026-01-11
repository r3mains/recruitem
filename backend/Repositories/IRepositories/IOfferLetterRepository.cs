using backend.DTOs.OfferLetter;

namespace backend.Repositories.IRepositories;

public interface IOfferLetterRepository
{
  Task<OfferLetterDto?> GetByIdAsync(Guid id);
  Task<OfferLetterDto?> GetOfferLetterByIdAsync(Guid id);
  Task<List<OfferLetterDto>> GetAllAsync();
  Task<List<OfferLetterDto>> GetByJobApplicationIdAsync(Guid jobApplicationId);
  Task<List<OfferLetterDto>> GetByStatusAsync(string status);
  Task<OfferLetterDto> CreateAsync(CreateOfferLetterDto dto, Guid createdBy);
  Task<OfferLetterDto?> UpdateAsync(Guid id, UpdateOfferLetterDto dto);
  Task<bool> DeleteAsync(Guid id);
  Task<bool> ExistsAsync(Guid id);
  Task<bool> AcceptOfferAsync(Guid id);
  Task<bool> RejectOfferAsync(Guid id, string reason);
  Task<bool> UpdatePdfPathAsync(Guid id, string pdfPath);
}
