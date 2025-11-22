using backend.DTOs.Interview;
using backend.Models;

namespace backend.Repositories.IRepositories
{
  public interface IInterviewRepository
  {
    Task<IEnumerable<InterviewDto>> GetAllInterviewsAsync();
    Task<InterviewDto?> GetInterviewByIdAsync(Guid id);
    Task<IEnumerable<InterviewDto>> GetInterviewsByJobApplicationIdAsync(Guid jobApplicationId);
    Task<InterviewDto> CreateInterviewAsync(CreateInterviewDto dto, Guid createdBy);
    Task<InterviewDto?> UpdateInterviewAsync(Guid id, UpdateInterviewDto dto);
    Task<bool> DeleteInterviewAsync(Guid id);

    Task<IEnumerable<InterviewScheduleDto>> GetSchedulesByInterviewIdAsync(Guid interviewId);
    Task<InterviewScheduleDto?> GetScheduleByIdAsync(Guid id);
    Task<InterviewScheduleDto> CreateScheduleAsync(CreateInterviewScheduleDto dto, Guid createdBy);
    Task<InterviewScheduleDto?> UpdateScheduleAsync(Guid id, UpdateInterviewScheduleDto dto);
    Task<bool> DeleteScheduleAsync(Guid id);

    Task<IEnumerable<InterviewFeedbackDto>> GetFeedbackByInterviewIdAsync(Guid interviewId);
    Task<InterviewFeedbackDto?> GetFeedbackByIdAsync(Guid id);
    Task<InterviewFeedbackDto> CreateFeedbackAsync(CreateInterviewFeedbackDto dto, Guid feedbackBy);
    Task<InterviewFeedbackDto?> UpdateFeedbackAsync(Guid id, UpdateInterviewFeedbackDto dto);
    Task<bool> DeleteFeedbackAsync(Guid id);

    Task<IEnumerable<InterviewType>> GetInterviewTypesAsync();
  }
}
