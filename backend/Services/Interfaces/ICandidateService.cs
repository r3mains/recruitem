using Backend.Dtos.Candidates;

namespace Backend.Services.Interfaces;

public interface ICandidateService
{
  Task<CandidateDto?> GetById(Guid id);
  Task<List<CandidateDto>> GetAll();
  Task<CandidateDto> Create(CandidateCreateDto dto);
  Task<CandidateDto?> Update(Guid id, CandidateUpdateDto dto);
  Task<bool> Delete(Guid id);
}
