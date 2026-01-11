using backend.DTOs.Event;

namespace backend.Repositories.IRepositories;

public interface IEventRepository
{
  Task<IEnumerable<EventListDto>> GetAllEventsAsync();
  Task<EventDto?> GetEventByIdAsync(Guid id);
  Task<EventDto> CreateEventAsync(CreateEventDto dto);
  Task<EventDto?> UpdateEventAsync(Guid id, UpdateEventDto dto);
  Task<bool> DeleteEventAsync(Guid id);
  
  Task<IEnumerable<EventCandidateDto>> GetEventCandidatesAsync(Guid eventId);
  Task<EventCandidateDto> RegisterCandidateAsync(RegisterCandidateToEventDto dto);
  Task<EventCandidateDto?> UpdateCandidateStatusAsync(Guid eventCandidateId, UpdateEventCandidateStatusDto dto);
  Task<bool> RemoveCandidateFromEventAsync(Guid eventCandidateId);
}
