using backend.DTOs.Verification;

namespace backend.Repositories.IRepositories;

public interface IVerificationRepository
{
  Task<IEnumerable<VerificationListDto>> GetAllVerificationsAsync();
  Task<IEnumerable<VerificationListDto>> GetVerificationsByCandidateAsync(Guid candidateId);
  Task<VerificationDto?> GetVerificationByIdAsync(Guid id);
  Task<VerificationDto> CreateVerificationAsync(CreateVerificationDto dto);
  Task<VerificationDto?> UpdateVerificationAsync(Guid id, UpdateVerificationDto dto);
  Task<bool> DeleteVerificationAsync(Guid id);
}
