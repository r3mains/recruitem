using Backend.Dtos.JobApplications;

namespace Backend.Services.Interfaces;

public interface IScreeningService
{
  Task<JobApplicationDto?> ScreenApplicationAsync(Guid applicationId, ScreeningDto screeningDto, Guid reviewerId);
  Task<List<JobApplicationDto>> BulkScreenApplicationsAsync(BulkScreeningDto bulkScreeningDto, Guid reviewerId);
  Task<List<JobApplicationDto>> ShortlistApplicationsAsync(ShortlistingDto shortlistingDto, Guid reviewerId);
  Task<List<JobApplicationDto>> GetApplicationsForScreeningAsync(Guid? jobId = null, Guid? statusId = null);
  Task<List<JobApplicationDto>> GetShortlistedApplicationsAsync(Guid? jobId = null);
  Task<double> CalculateApplicationScoreAsync(Guid applicationId);
  Task<List<JobApplicationDto>> GetApplicationsByScoreRangeAsync(Guid jobId, double minScore, double maxScore);
}
