using backend.DTOs;
using backend.Models;
using backend.Repositories.IRepositories;
using backend.Consts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System.Linq.Expressions;

namespace backend.Repositories;

public class UserRepository(UserManager<User> userManager, IMapper mapper, IProfileRepository profileRepository) : IUserRepository
{
  private readonly UserManager<User> _userManager = userManager;
  private readonly IMapper _mapper = mapper;
  private readonly IProfileRepository _profileRepository = profileRepository;

  public async Task<PagedResultDto<UserListDto>> GetUsersAsync(UserQueryDto query)
  {
    var usersQuery = _userManager.Users.Where(u => !u.IsDeleted);

    if (!string.IsNullOrEmpty(query.SearchTerm))
    {
      var searchTerm = query.SearchTerm.ToLower();
      usersQuery = usersQuery.Where(u =>
        u.Email!.ToLower().Contains(searchTerm) ||
        u.UserName!.ToLower().Contains(searchTerm));
    }

    if (query.EmailConfirmed.HasValue)
    {
      usersQuery = usersQuery.Where(u => u.EmailConfirmed == query.EmailConfirmed.Value);
    }

    if (query.IsActive.HasValue)
    {
      if (query.IsActive.Value)
      {
        usersQuery = usersQuery.Where(u => !u.LockoutEnabled || u.LockoutEnd == null || u.LockoutEnd <= DateTimeOffset.Now);
      }
      else
      {
        usersQuery = usersQuery.Where(u => u.LockoutEnabled && u.LockoutEnd != null && u.LockoutEnd > DateTimeOffset.Now);
      }
    }

    usersQuery = ApplySorting(usersQuery, query.SortBy, query.SortDescending);

    var totalCount = await usersQuery.CountAsync();
    var totalPages = (int)Math.Ceiling((double)totalCount / query.PageSize);

    var users = await usersQuery
      .Skip((query.Page - 1) * query.PageSize)
      .Take(query.PageSize)
      .ToListAsync();

    var userDtos = new List<UserListDto>();
    foreach (var user in users)
    {
      var roles = await _userManager.GetRolesAsync(user);

      if (!string.IsNullOrEmpty(query.Role) && !roles.Contains(query.Role))
        continue;

      userDtos.Add(new UserListDto(
        user.Id,
        user.Email,
        user.UserName,
        user.EmailConfirmed,
        user.CreatedAt,
        roles
      ));
    }

    return new PagedResultDto<UserListDto>(
      userDtos,
      totalCount,
      query.Page,
      query.PageSize,
      totalPages
    );
  }

  public async Task<UserWithRolesDto?> GetUserByIdAsync(string userId)
  {
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null || user.IsDeleted)
      return null;

    var roles = await _userManager.GetRolesAsync(user);
    return new UserWithRolesDto(
      user.Id,
      user.Email,
      user.UserName,
      user.EmailConfirmed,
      user.LockoutEnabled,
      user.LockoutEnd,
      user.AccessFailedCount,
      user.CreatedAt,
      user.UpdatedAt,
      user.IsDeleted,
      roles
    );
  }

  public async Task<UserWithRolesDto?> GetUserByEmailAsync(string email)
  {
    var user = await _userManager.FindByEmailAsync(email);
    if (user == null || user.IsDeleted)
      return null;

    var roles = await _userManager.GetRolesAsync(user);
    return new UserWithRolesDto(
      user.Id,
      user.Email,
      user.UserName,
      user.EmailConfirmed,
      user.LockoutEnabled,
      user.LockoutEnd,
      user.AccessFailedCount,
      user.CreatedAt,
      user.UpdatedAt,
      user.IsDeleted,
      roles
    );
  }

  public async Task<IdentityResult> CreateUserAsync(CreateUserDto createUserDto, string defaultRole = Roles.Candidate)
  {
    var user = new User
    {
      Email = createUserDto.Email,
      UserName = createUserDto.UserName ?? createUserDto.Email,
      CreatedAt = DateTimeOffset.Now,
      UpdatedAt = DateTimeOffset.Now,
      IsDeleted = false
    };

    var result = await _userManager.CreateAsync(user, createUserDto.Password);
    if (!result.Succeeded)
      return result;

    var roleResult = await _userManager.AddToRoleAsync(user, defaultRole);
    if (!roleResult.Succeeded)
    {
      await _userManager.DeleteAsync(user);
      return roleResult;
    }

    return IdentityResult.Success;
  }

  public async Task<IdentityResult> UpdateUserAsync(string userId, UpdateUserDto updateUserDto)
  {
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null || user.IsDeleted)
      return IdentityResult.Failed(new IdentityError { Description = "User not found" });

    if (!string.IsNullOrEmpty(updateUserDto.UserName))
      user.UserName = updateUserDto.UserName;

    if (!string.IsNullOrEmpty(updateUserDto.Email))
      user.Email = updateUserDto.Email;

    user.UpdatedAt = DateTimeOffset.Now;

    return await _userManager.UpdateAsync(user);
  }

  public async Task<IdentityResult> DeleteUserAsync(string userId)
  {
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null)
      return IdentityResult.Failed(new IdentityError { Description = "User not found" });

    user.IsDeleted = true;
    user.UpdatedAt = DateTimeOffset.Now;

    return await _userManager.UpdateAsync(user);
  }

  public async Task<IdentityResult> RestoreUserAsync(string userId)
  {
    var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
    if (user == null)
      return IdentityResult.Failed(new IdentityError { Description = "User not found" });

    user.IsDeleted = false;
    user.UpdatedAt = DateTimeOffset.Now;

    return await _userManager.UpdateAsync(user);
  }

  public async Task<IList<string>> GetUserRolesAsync(string userId)
  {
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null || user.IsDeleted)
      return new List<string>();

    return await _userManager.GetRolesAsync(user);
  }

  public async Task<IdentityResult> AssignRoleAsync(AssignRoleDto assignRoleDto)
  {
    var user = await _userManager.FindByIdAsync(assignRoleDto.UserId);
    if (user == null || user.IsDeleted)
      return IdentityResult.Failed(new IdentityError { Description = "User not found" });

    var currentRoles = await _userManager.GetRolesAsync(user);
    var result = await _userManager.AddToRoleAsync(user, assignRoleDto.Role);

    if (result.Succeeded)
    {
      var newRoles = await _userManager.GetRolesAsync(user);
      await _profileRepository.HandleRoleChangeAsync(assignRoleDto.UserId, newRoles, currentRoles);
    }

    return result;
  }

  public async Task<IdentityResult> RemoveRoleAsync(RemoveRoleDto removeRoleDto)
  {
    var user = await _userManager.FindByIdAsync(removeRoleDto.UserId);
    if (user == null || user.IsDeleted)
      return IdentityResult.Failed(new IdentityError { Description = "User not found" });

    var currentRoles = await _userManager.GetRolesAsync(user);
    var result = await _userManager.RemoveFromRoleAsync(user, removeRoleDto.Role);

    if (result.Succeeded)
    {
      var newRoles = await _userManager.GetRolesAsync(user);
      await _profileRepository.HandleRoleChangeAsync(removeRoleDto.UserId, newRoles, currentRoles);
    }

    return result;
  }

  public async Task<IdentityResult> UpdateUserRolesAsync(UpdateUserRolesDto updateRolesDto)
  {
    var user = await _userManager.FindByIdAsync(updateRolesDto.UserId);
    if (user == null || user.IsDeleted)
      return IdentityResult.Failed(new IdentityError { Description = "User not found" });

    var currentRoles = await _userManager.GetRolesAsync(user);

    var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
    if (!removeResult.Succeeded)
      return removeResult;

    var addResult = await _userManager.AddToRolesAsync(user, updateRolesDto.Roles);
    if (!addResult.Succeeded)
      return addResult;

    await _profileRepository.HandleRoleChangeAsync(updateRolesDto.UserId, updateRolesDto.Roles, currentRoles);

    return IdentityResult.Success;
  }

  public async Task<IdentityResult> LockUserAsync(string userId, DateTimeOffset? lockoutEnd = null)
  {
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null || user.IsDeleted)
      return IdentityResult.Failed(new IdentityError { Description = "User not found" });

    var lockoutEndDate = lockoutEnd ?? DateTimeOffset.Now.AddYears(100);
    return await _userManager.SetLockoutEndDateAsync(user, lockoutEndDate);
  }

  public async Task<IdentityResult> UnlockUserAsync(string userId)
  {
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null || user.IsDeleted)
      return IdentityResult.Failed(new IdentityError { Description = "User not found" });

    return await _userManager.SetLockoutEndDateAsync(user, null);
  }

  public async Task<IdentityResult> ConfirmUserEmailAsync(string userId)
  {
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null || user.IsDeleted)
      return IdentityResult.Failed(new IdentityError { Description = "User not found" });

    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
    return await _userManager.ConfirmEmailAsync(user, token);
  }

  public async Task<int> GetTotalUsersCountAsync()
  {
    return await _userManager.Users.CountAsync(u => !u.IsDeleted);
  }

  public async Task<int> GetActiveUsersCountAsync()
  {
    return await _userManager.Users.CountAsync(u =>
      !u.IsDeleted &&
      (!u.LockoutEnabled || u.LockoutEnd == null || u.LockoutEnd <= DateTimeOffset.Now));
  }

  public async Task<Dictionary<string, int>> GetUsersByRoleCountAsync()
  {
    var roleCount = new Dictionary<string, int>();
    var roles = new[] { Roles.Admin, Roles.HR, Roles.Recruiter, Roles.Interviewer, Roles.Reviewer, Roles.Candidate, Roles.Viewer };

    foreach (var role in roles)
    {
      var usersInRole = await _userManager.GetUsersInRoleAsync(role);
      roleCount[role] = usersInRole.Count(u => !u.IsDeleted);
    }

    return roleCount;
  }

  private static IQueryable<User> ApplySorting(IQueryable<User> query, string? sortBy, bool sortDescending)
  {
    if (string.IsNullOrEmpty(sortBy))
      sortBy = "CreatedAt";

    Expression<Func<User, object>> sortExpression = sortBy.ToLower() switch
    {
      "id" => u => u.Id,
      "email" => u => u.Email!,
      "username" => u => u.UserName!,
      "createdat" => u => u.CreatedAt!,
      "updatedat" => u => u.UpdatedAt!,
      _ => u => u.CreatedAt!
    };

    return sortDescending
      ? query.OrderByDescending(sortExpression)
      : query.OrderBy(sortExpression);
  }
}
