using Backend.Dtos.Jobs;

namespace Backend.Services.Interfaces;

public interface IJobService
{
  Task<JobDto?> GetById(Guid id);
  Task<List<JobDto>> GetAll(Guid? recruiterId = null, Guid? statusId = null, Guid? positionId = null);
  Task<JobDto> Create(JobCreateDto dto);
  Task<JobDto?> Update(Guid id, JobUpdateDto dto);
  Task<bool> Delete(Guid id);
}
