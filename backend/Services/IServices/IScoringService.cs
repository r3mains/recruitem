using backend.Models;

namespace backend.Services.IServices;

public interface IScoringService
{
  Task<AutomatedScore> CalculateScoreAsync(Guid jobApplicationId);
  Task<AutomatedScore?> GetScoreAsync(Guid jobApplicationId);
  Task<IEnumerable<AutomatedScore>> GetScoresByJobAsync(Guid jobId);
  Task<ScoringConfiguration?> GetScoringConfigurationAsync(Guid positionId);
  Task<ScoringConfiguration> CreateOrUpdateScoringConfigurationAsync(ScoringConfiguration config);
}
