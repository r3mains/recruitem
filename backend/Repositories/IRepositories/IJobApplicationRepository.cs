namespace backend.Repositories.IRepositories;

public interface IJobApplicationRepository
{
  Task<IEnumerable<backend.Models.JobApplication>> GetAllAsync(
    string? search = null,
    int page = 1,
    int pageSize = 10,
    Guid? jobId = null,
    Guid? candidateId = null,
    Guid? statusId = null);

  Task<backend.Models.JobApplication?> GetByIdAsync(Guid id);
  Task<backend.Models.JobApplication> CreateAsync(backend.Models.JobApplication jobApplication);
  Task<backend.Models.JobApplication> UpdateAsync(backend.Models.JobApplication jobApplication);
  Task DeleteAsync(Guid id);
  Task<bool> HasAppliedAsync(Guid candidateId, Guid jobId);
  Task<int> GetCountAsync(string? search = null, Guid? jobId = null, Guid? candidateId = null);

  Task<IEnumerable<backend.Models.JobApplication>> GetByIdsAsync(IEnumerable<Guid> applicationIds);
  Task UpdateMultipleAsync(IEnumerable<backend.Models.JobApplication> applications);

  Task<Dictionary<string, int>> GetApplicationStatsByJobAsync(Guid jobId);
  Task<Dictionary<string, int>> GetApplicationStatsByCandidateAsync(Guid candidateId);

  Task AddStatusHistoryAsync(backend.Models.ApplicationStatusHistory statusHistory);
}
