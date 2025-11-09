using backend.DTOs;
using backend.Models;
using backend.Consts;
using Microsoft.AspNetCore.Identity;

namespace backend.Repositories.IRepositories;

public interface IUserRepository
{
  Task<PagedResultDto<UserListDto>> GetUsersAsync(UserQueryDto query);
  Task<UserWithRolesDto?> GetUserByIdAsync(string userId);
  Task<UserWithRolesDto?> GetUserByEmailAsync(string email);
  Task<IdentityResult> CreateUserAsync(CreateUserDto createUserDto, string defaultRole = Roles.Candidate);
  Task<IdentityResult> UpdateUserAsync(string userId, UpdateUserDto updateUserDto);
  Task<IdentityResult> DeleteUserAsync(string userId);
  Task<IdentityResult> RestoreUserAsync(string userId);

  Task<IList<string>> GetUserRolesAsync(string userId);
  Task<IdentityResult> AssignRoleAsync(AssignRoleDto assignRoleDto);
  Task<IdentityResult> RemoveRoleAsync(RemoveRoleDto removeRoleDto);
  Task<IdentityResult> UpdateUserRolesAsync(UpdateUserRolesDto updateRolesDto);

  Task<IdentityResult> LockUserAsync(string userId, DateTimeOffset? lockoutEnd = null);
  Task<IdentityResult> UnlockUserAsync(string userId);
  Task<IdentityResult> ConfirmUserEmailAsync(string userId);

  Task<int> GetTotalUsersCountAsync();
  Task<int> GetActiveUsersCountAsync();
  Task<Dictionary<string, int>> GetUsersByRoleCountAsync();
}
