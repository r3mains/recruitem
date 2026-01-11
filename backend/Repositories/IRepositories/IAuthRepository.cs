using backend.DTOs;
using backend.Models;
using backend.Consts;
using Microsoft.AspNetCore.Identity;

namespace backend.Repositories.IRepositories;

public interface IAuthRepository
{
  Task<IdentityResult> RegisterUserAsync(RegisterDto registerDto, string role = Roles.Candidate);
  Task<(bool Success, string Token, IList<string> Roles)> LoginAsync(LoginDto loginDto);
  Task<User?> GetUserByIdAsync(string userId);
  Task<User?> GetUserByEmailAsync(string email);

  Task<(bool Success, string Token, string UserId)> GeneratePasswordResetTokenAsync(string email);
  Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
  Task<IdentityResult> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto);

  Task<(bool Success, string Token, string UserId)> GenerateEmailConfirmationTokenAsync(string email);
  Task<IdentityResult> ConfirmEmailAsync(ConfirmEmailDto confirmEmailDto);

  Task<IList<string>> GetUserRolesAsync(string userId);
  Task<IdentityResult> AssignRoleAsync(string userId, string role);
  Task<IdentityResult> RemoveFromRoleAsync(string userId, string role);

  Task<object?> GetUserProfileAsync(string userId);
}
