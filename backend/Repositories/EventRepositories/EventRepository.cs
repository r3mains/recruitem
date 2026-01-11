using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;
using backend.DTOs.Event;
using backend.Repositories.IRepositories;

namespace backend.Repositories;

public class EventRepository(ApplicationDbContext context) : IEventRepository
{
  private readonly ApplicationDbContext _context = context;

  public async Task<IEnumerable<EventListDto>> GetAllEventsAsync()
  {
    return await _context.Events
      .Include(e => e.EventCandidates)
      .Where(e => !e.IsDeleted)
      .OrderByDescending(e => e.Date)
      .Select(e => new EventListDto(
        e.Id,
        e.Name,
        e.Type,
        e.Location,
        e.Date,
        e.EventCandidates.Count,
        e.CreatedAt
      ))
      .ToListAsync();
  }

  public async Task<EventDto?> GetEventByIdAsync(Guid id)
  {
    return await _context.Events
      .Include(e => e.CreatedByEmployee)
        .ThenInclude(emp => emp.User)
      .Where(e => e.Id == id && !e.IsDeleted)
      .Select(e => new EventDto(
        e.Id,
        e.Name,
        e.Type,
        e.Location,
        e.Date,
        e.CreatedBy,
        e.CreatedByEmployee.User.UserName,
        e.CreatedAt,
        e.UpdatedAt,
        e.IsDeleted
      ))
      .FirstOrDefaultAsync();
  }

  public async Task<EventDto> CreateEventAsync(CreateEventDto dto)
  {
    var eventEntity = new Event
    {
      Id = Guid.NewGuid(),
      Name = dto.Name,
      Type = dto.Type,
      Location = dto.Location,
      Date = dto.Date,
      CreatedBy = dto.CreatedBy,
      CreatedAt = DateTime.UtcNow,
      IsDeleted = false
    };

    _context.Events.Add(eventEntity);
    await _context.SaveChangesAsync();

    return (await GetEventByIdAsync(eventEntity.Id))!;
  }

  public async Task<EventDto?> UpdateEventAsync(Guid id, UpdateEventDto dto)
  {
    var eventEntity = await _context.Events
      .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

    if (eventEntity == null)
      return null;

    eventEntity.Name = dto.Name;
    eventEntity.Type = dto.Type;
    eventEntity.Location = dto.Location;
    eventEntity.Date = dto.Date;
    eventEntity.UpdatedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync();

    return await GetEventByIdAsync(id);
  }

  public async Task<bool> DeleteEventAsync(Guid id)
  {
    var eventEntity = await _context.Events
      .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

    if (eventEntity == null)
      return false;

    eventEntity.IsDeleted = true;
    eventEntity.UpdatedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync();
    return true;
  }

  public async Task<IEnumerable<EventCandidateDto>> GetEventCandidatesAsync(Guid eventId)
  {
    return await _context.EventCandidates
      .Include(ec => ec.Event)
      .Include(ec => ec.Candidate)
        .ThenInclude(c => c.User)
      .Include(ec => ec.Status)
      .Where(ec => ec.EventId == eventId)
      .Select(ec => new EventCandidateDto(
        ec.Id,
        ec.EventId,
        ec.Event.Name,
        ec.CandidateId,
        ec.Candidate.User.UserName ?? "Unknown",
        ec.StatusId,
        ec.Status.Status,
        ec.RegisteredAt,
        ec.UpdatedAt
      ))
      .ToListAsync();
  }

  public async Task<EventCandidateDto> RegisterCandidateAsync(RegisterCandidateToEventDto dto)
  {
    var eventCandidate = new EventCandidate
    {
      Id = Guid.NewGuid(),
      EventId = dto.EventId,
      CandidateId = dto.CandidateId,
      StatusId = dto.StatusId,
      RegisteredAt = DateTime.UtcNow
    };

    _context.EventCandidates.Add(eventCandidate);
    await _context.SaveChangesAsync();

    return (await _context.EventCandidates
      .Include(ec => ec.Event)
      .Include(ec => ec.Candidate)
        .ThenInclude(c => c.User)
      .Include(ec => ec.Status)
      .Where(ec => ec.Id == eventCandidate.Id)
      .Select(ec => new EventCandidateDto(
        ec.Id,
        ec.EventId,
        ec.Event.Name,
        ec.CandidateId,
        ec.Candidate.User.UserName ?? "Unknown",
        ec.StatusId,
        ec.Status.Status,
        ec.RegisteredAt,
        ec.UpdatedAt
      ))
      .FirstAsync())!;
  }

  public async Task<EventCandidateDto?> UpdateCandidateStatusAsync(Guid eventCandidateId, UpdateEventCandidateStatusDto dto)
  {
    var eventCandidate = await _context.EventCandidates
      .FirstOrDefaultAsync(ec => ec.Id == eventCandidateId);

    if (eventCandidate == null)
      return null;

    eventCandidate.StatusId = dto.StatusId;
    eventCandidate.UpdatedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync();

    return await _context.EventCandidates
      .Include(ec => ec.Event)
      .Include(ec => ec.Candidate)
        .ThenInclude(c => c.User)
      .Include(ec => ec.Status)
      .Where(ec => ec.Id == eventCandidateId)
      .Select(ec => new EventCandidateDto(
        ec.Id,
        ec.EventId,
        ec.Event.Name,
        ec.CandidateId,
        ec.Candidate.User.UserName ?? "Unknown",
        ec.StatusId,
        ec.Status.Status,
        ec.RegisteredAt,
        ec.UpdatedAt
      ))
      .FirstOrDefaultAsync();
  }

  public async Task<bool> RemoveCandidateFromEventAsync(Guid eventCandidateId)
  {
    var eventCandidate = await _context.EventCandidates
      .FirstOrDefaultAsync(ec => ec.Id == eventCandidateId);

    if (eventCandidate == null)
      return false;

    _context.EventCandidates.Remove(eventCandidate);
    await _context.SaveChangesAsync();
    return true;
  }
}
