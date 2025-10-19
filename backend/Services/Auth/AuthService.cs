using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Linq;
using Backend.Dtos.Auth;
using Backend.Models;
using Backend.Repositories.Interfaces;
using Backend.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Services.Auth;

public class AuthService(IUserRepository userRepo, IConfiguration config) : IAuthService
{
  public async Task<AuthResponseDto?> Login(LoginRequestDto dto)
  {
    var users = await userRepo.GetAll();
    var user = users.FirstOrDefault(u => u.Email == dto.Email);
    if (user == null) return null;
    if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password)) return null;

    var userWithRelations = await userRepo.GetById(user.Id);
    return GenerateToken(userWithRelations!);
  }

  public async Task<AuthResponseDto> Register(RegisterRequestDto dto)
  {
    var existing = (await userRepo.GetAll()).Any(u => u.Email == dto.Email);
    if (existing) throw new Exception("Email already used");

    var hash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
    var user = new User { Id = Guid.NewGuid(), Email = dto.Email, Password = hash, RoleId = dto.RoleId, CreatedAt = DateTime.UtcNow };
    await userRepo.Add(user);
    return GenerateToken(user);
  }

  private AuthResponseDto GenerateToken(User user)
  {
    var key = config["Jwt:Key"] ?? throw new Exception("Jwt:Key missing");

    var claims = new List<Claim>
    {
      new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
      new(JwtRegisteredClaimNames.Email, user.Email),
      new("role", user.Role?.Name ?? "Unknown")
    };

    if (user.Candidate != null)
    {
      claims.Add(new("CandidateId", user.Candidate.Id.ToString()));
    }

    if (user.Employee != null)
    {
      claims.Add(new("EmployeeId", user.Employee.Id.ToString()));
    }

    var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256);
    var token = new JwtSecurityToken(
      claims: claims,
      expires: DateTime.UtcNow.AddMinutes(60),
      signingCredentials: creds
    );

    var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);
    return new AuthResponseDto { AccessToken = tokenStr, ExpiresAtUtc = token.ValidTo };
  }
}
