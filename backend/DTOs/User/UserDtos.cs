namespace backend.DTOs;

// Request DTOs
public record CreateUserDto(
  string Email,
  string Password,
  string? UserName = null
);

public record UpdateUserDto(
  string? UserName,
  string? Email = null
);

public record AssignRoleDto(
  string UserId,
  string Role
);

public record RemoveRoleDto(
  string UserId,
  string Role
);

public record UpdateUserRolesDto(
  string UserId,
  IList<string> Roles
);

// Response DTOs
public record UserDto(
  Guid Id,
  string? Email,
  string? UserName,
  bool EmailConfirmed,
  bool LockoutEnabled,
  DateTimeOffset? LockoutEnd,
  int AccessFailedCount,
  DateTimeOffset? CreatedAt,
  DateTimeOffset? UpdatedAt,
  bool IsDeleted
);

public record UserWithRolesDto(
  Guid Id,
  string? Email,
  string? UserName,
  bool EmailConfirmed,
  bool LockoutEnabled,
  DateTimeOffset? LockoutEnd,
  int AccessFailedCount,
  DateTimeOffset? CreatedAt,
  DateTimeOffset? UpdatedAt,
  bool IsDeleted,
  IList<string> Roles
);

public record UserListDto(
  Guid Id,
  string? Email,
  string? UserName,
  bool EmailConfirmed,
  DateTimeOffset? CreatedAt,
  IList<string> Roles
);

public record UserQueryDto(
  string? SearchTerm = null,
  string? Role = null,
  bool? EmailConfirmed = null,
  bool? IsActive = null,
  int Page = 1,
  int PageSize = 10,
  string? SortBy = "CreatedAt",
  bool SortDescending = true
);

public record PagedResultDto<T>(
  IList<T> Items,
  int TotalCount,
  int Page,
  int PageSize,
  int TotalPages
);
