using Backend.Models;

namespace Backend.Repositories.Interfaces;

public interface IJobApplicationRepository
{
  Task<List<JobApplication>> GetAll(Guid? jobId = null, Guid? candidateId = null, Guid? statusId = null);
  Task<JobApplication?> GetById(Guid id);
  Task<JobApplication> Add(JobApplication jobApplication);
  Task<JobApplication?> Update(Guid id, JobApplication jobApplication);
  Task<bool> Delete(Guid id);
  Task<bool> ExistsForJobAndCandidate(Guid jobId, Guid candidateId);
}
