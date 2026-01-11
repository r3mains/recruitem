namespace backend.DTOs.Event;

public record CreateEventDto(
  string Name,
  string? Type,
  string? Location,
  DateTime Date,
  Guid CreatedBy
);

public record UpdateEventDto(
  string Name,
  string? Type,
  string? Location,
  DateTime Date
);

public record EventDto(
  Guid Id,
  string Name,
  string? Type,
  string? Location,
  DateTime Date,
  Guid CreatedBy,
  string? CreatedByName,
  DateTime CreatedAt,
  DateTime? UpdatedAt,
  bool IsDeleted
);

public record EventListDto(
  Guid Id,
  string Name,
  string? Type,
  string? Location,
  DateTime Date,
  int CandidateCount,
  DateTime CreatedAt
);

public record RegisterCandidateToEventDto(
  Guid EventId,
  Guid CandidateId,
  Guid StatusId
);

public record UpdateEventCandidateStatusDto(
  Guid StatusId
);

public record EventCandidateDto(
  Guid Id,
  Guid EventId,
  string EventName,
  Guid CandidateId,
  string CandidateName,
  Guid StatusId,
  string Status,
  DateTime RegisteredAt,
  DateTime? UpdatedAt
);
