using Backend.Dtos.Candidates;

namespace Backend.Services.Interfaces;

public interface ICandidateService
{
  Task<CandidateDto?> GetById(Guid id);
  Task<CandidateDto?> GetByUserId(Guid userId);
  Task<List<CandidateDto>> GetAll();
  Task<CandidateSearchResultDto> Search(string? search, string? skills, int page, int limit);
  Task<CandidateDto> Create(CandidateCreateDto dto);
  Task<CandidateDto?> Update(Guid id, CandidateUpdateDto dto);
  Task<bool> Delete(Guid id);

  Task<CandidateDto> CreateCandidateProfile(CreateCandidateDto dto);
  Task<CandidateDto?> UpdateCandidateProfile(Guid id, UpdateCandidateDto dto);
  Task<List<CandidateDto>> SearchCandidates(CandidateSearchDto searchDto);
  Task<List<CandidateDto>> GetCandidatesBySkills(List<Guid> skillIds, int? minExperience = null);

  Task<List<CandidateSkillDto>> GetCandidateSkills(Guid candidateId);
  Task<CandidateSkillDto> AddCandidateSkill(Guid candidateId, CandidateSkillCreateDto dto);
  Task<CandidateSkillDto?> UpdateCandidateSkill(Guid candidateId, Guid skillId, CandidateSkillUpdateDto dto);
  Task<bool> RemoveCandidateSkill(Guid candidateId, Guid skillId);
}
