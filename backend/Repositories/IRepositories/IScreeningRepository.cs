using backend.Models;

namespace backend.Repositories.IRepositories;

public interface IScreeningRepository
{
  // Resume screening
  Task<JobApplication> ScreenResumeAsync(Guid jobApplicationId, decimal? score, string? comments, bool approved, Guid screenerId);
  Task<IEnumerable<JobApplication>> GetApplicationsForScreeningAsync(Guid? positionId = null, string? search = null, int page = 1, int pageSize = 10);
  
  // Comments
  Task<Comment> AddCommentAsync(Guid jobApplicationId, string comment, Guid commenterId);
  Task<IEnumerable<Comment>> GetApplicationCommentsAsync(Guid jobApplicationId);
  
  // Skills management
  Task UpdateCandidateSkillsAsync(Guid candidateId, List<(Guid SkillId, int? YearsOfExperience, string? ProficiencyLevel)> skills, Guid updatedBy);
  Task<IEnumerable<CandidateSkill>> GetCandidateSkillsForScreeningAsync(Guid candidateId);
  
  // Position reviewer assignment
  Task<Position> AssignReviewerToPositionAsync(Guid positionId, Guid reviewerId);
  Task<IEnumerable<Position>> GetPositionsForReviewerAssignmentAsync();
  
  // Shortlisting
  Task<JobApplication> ShortlistCandidateAsync(Guid jobApplicationId, string? comments, Guid shortlistedBy);
  Task<IEnumerable<JobApplication>> GetShortlistedCandidatesAsync(Guid? positionId = null, int page = 1, int pageSize = 10);
  
  // Statistics and reports
  Task<Dictionary<string, int>> GetScreeningStatsAsync(Guid? positionId = null, DateTime? fromDate = null, DateTime? toDate = null);
  Task<int> GetPendingScreeningCountAsync(Guid? reviewerId = null);
}
