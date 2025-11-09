using backend.Models;
using backend.DTOs.Job;

namespace backend.Repositories.IRepositories
{
  public interface IJobRepository
  {
    Task<Job> CreateJobAsync(Job job);
    Task<Job?> GetJobByIdAsync(Guid id);
    Task<JobResponseDto?> GetJobDetailsByIdAsync(Guid id);
    Task<IEnumerable<JobListDto>> GetJobsAsync(int page = 1, int pageSize = 10, string? search = null, Guid? statusId = null, Guid? jobTypeId = null);
    Task<Job> UpdateJobAsync(Job job);
    Task DeleteJobAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<IEnumerable<JobListDto>> GetJobsByRecruiterAsync(Guid recruiterId, int page = 1, int pageSize = 10);
    Task<IEnumerable<JobListDto>> GetJobsByPositionAsync(Guid positionId);
    Task<int> GetJobCountAsync(Guid? statusId = null, Guid? recruiterId = null);
    Task<bool> CanUpdateJobAsync(Guid jobId, Guid userId);
    Task<bool> CanDeleteJobAsync(Guid jobId, Guid userId);
  }
}
