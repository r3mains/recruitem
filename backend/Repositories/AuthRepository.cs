using backend.DTOs;
using backend.Models;
using backend.Repositories.IRepositories;
using backend.Services.IServices;
using backend.Consts;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace backend.Repositories;

public class AuthRepository(UserManager<User> userManager, IMapper mapper, IConfiguration configuration, IEmailService emailService) : IAuthRepository
{
  private readonly UserManager<User> _userManager = userManager;
  private readonly IMapper _mapper = mapper;
  private readonly IConfiguration _configuration = configuration;
  private readonly IEmailService _emailService = emailService;

  public async Task<IdentityResult> RegisterUserAsync(RegisterDto registerDto, string role = Roles.Candidate)
  {
    var user = _mapper.Map<User>(registerDto);

    var createResult = await _userManager.CreateAsync(user, registerDto.Password);
    if (!createResult.Succeeded)
      return createResult;

    var roleResult = await _userManager.AddToRoleAsync(user, role);
    if (!roleResult.Succeeded)
    {
      await _userManager.DeleteAsync(user);
      return roleResult;
    }

    var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
    await _emailService.SendEmailConfirmationAsync(user.Email!, confirmationToken, user.UserName ?? "User");

    return IdentityResult.Success;
  }

  public async Task<(bool Success, string Token, IList<string> Roles)> LoginAsync(LoginDto loginDto)
  {
    var user = await _userManager.FindByEmailAsync(loginDto.Email);
    if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
    {
      return (false, string.Empty, new List<string>());
    }

    var roles = await _userManager.GetRolesAsync(user);
    var token = GenerateJwtToken(user, roles);

    return (true, token, roles);
  }

  public async Task<User?> GetUserByIdAsync(string userId)
  {
    return await _userManager.FindByIdAsync(userId);
  }
  public async Task<User?> GetUserByEmailAsync(string email)
  {
    return await _userManager.FindByEmailAsync(email);
  }

  public async Task<(bool Success, string Token, string UserId)> GeneratePasswordResetTokenAsync(string email)
  {
    var user = await _userManager.FindByEmailAsync(email);
    if (user == null)
    {
      return (false, string.Empty, string.Empty);
    }

    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

    await _emailService.SendPasswordResetEmailAsync(email, token, user.UserName ?? "User");

    return (true, token, user.Id.ToString());
  }
  public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
  {
    var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
    if (user == null)
    {
      return IdentityResult.Failed(new IdentityError { Description = "User not found" });
    }

    return await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
  }

  public async Task<IdentityResult> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto)
  {
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null)
    {
      return IdentityResult.Failed(new IdentityError { Description = "User not found" });
    }

    return await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
  }

  public async Task<(bool Success, string Token, string UserId)> GenerateEmailConfirmationTokenAsync(string email)
  {
    var user = await _userManager.FindByEmailAsync(email);
    if (user == null)
    {
      return (false, string.Empty, string.Empty);
    }

    if (user.EmailConfirmed)
    {
      return (false, string.Empty, string.Empty);
    }

    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

    await _emailService.SendEmailConfirmationAsync(email, token, user.UserName ?? "User");

    return (true, token, user.Id.ToString());
  }
  public async Task<IdentityResult> ConfirmEmailAsync(ConfirmEmailDto confirmEmailDto)
  {
    var user = await _userManager.FindByIdAsync(confirmEmailDto.UserId);
    if (user == null)
    {
      return IdentityResult.Failed(new IdentityError { Description = "User not found" });
    }

    return await _userManager.ConfirmEmailAsync(user, confirmEmailDto.Token);
  }

  public async Task<IList<string>> GetUserRolesAsync(string userId)
  {
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null)
      return [];

    return await _userManager.GetRolesAsync(user);
  }

  public async Task<IdentityResult> AssignRoleAsync(string userId, string role)
  {
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null)
    {
      return IdentityResult.Failed(new IdentityError { Description = "User not found" });
    }

    return await _userManager.AddToRoleAsync(user, role);
  }

  public async Task<IdentityResult> RemoveFromRoleAsync(string userId, string role)
  {
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null)
    {
      return IdentityResult.Failed(new IdentityError { Description = "User not found" });
    }

    return await _userManager.RemoveFromRoleAsync(user, role);
  }

  public async Task<object?> GetUserProfileAsync(string userId)
  {
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null)
      return null;

    var roles = await _userManager.GetRolesAsync(user);
    var response = _mapper.Map<AuthResponseDto>(user);

    return new
    {
      response.Id,
      response.Email,
      response.UserName,
      roles,
      createdAt = user.CreatedAt,
      updatedAt = user.UpdatedAt
    };
  }

  private string GenerateJwtToken(User user, IList<string> roles)
  {
    var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email!)
        };

    foreach (var role in roles)
    {
      claims.Add(new Claim(ClaimTypes.Role, role));
    }

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: _configuration["Jwt:Issuer"],
        audience: _configuration["Jwt:Audience"],
        claims: claims,
        expires: DateTime.UtcNow.AddHours(1),
        signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}
