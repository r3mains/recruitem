namespace backend.Repositories.IRepositories;

public interface ICandidateRepository
{
  Task<IEnumerable<backend.Models.Candidate>> GetAllAsync(string? search = null, int page = 1, int pageSize = 10);
  Task<backend.Models.Candidate?> GetByIdAsync(Guid id);
  Task<backend.Models.Candidate?> GetByUserIdAsync(Guid userId);
  Task<backend.Models.Candidate?> GetByEmailAsync(string email);
  Task<backend.Models.Candidate> CreateAsync(backend.Models.Candidate candidate);
  Task<backend.Models.Candidate> UpdateAsync(backend.Models.Candidate candidate);
  Task DeleteAsync(Guid id);
  Task<bool> ExistsByEmailAsync(string email, Guid? excludeId = null);
  Task<int> GetCountAsync(string? search = null);

  Task<IEnumerable<backend.Models.CandidateSkill>> GetCandidateSkillsAsync(Guid candidateId);
  Task<backend.Models.CandidateSkill> AddSkillAsync(backend.Models.CandidateSkill candidateSkill);
  Task UpdateSkillAsync(backend.Models.CandidateSkill candidateSkill);
  Task RemoveSkillAsync(Guid candidateId, Guid skillId);
  Task RemoveAllSkillsAsync(Guid candidateId);

  Task<IEnumerable<backend.Models.CandidateQualification>> GetCandidateQualificationsAsync(Guid candidateId);
  Task<backend.Models.CandidateQualification> AddQualificationAsync(backend.Models.CandidateQualification candidateQualification);
  Task UpdateQualificationAsync(backend.Models.CandidateQualification candidateQualification);
  Task RemoveQualificationAsync(Guid candidateId, Guid qualificationId);
  Task RemoveAllQualificationsAsync(Guid candidateId);
}
