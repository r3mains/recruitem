using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Linq;
using Backend.Dtos.Auth;
using Backend.Models;
using Backend.Repositories.Interfaces;
using Backend.Services.Interfaces;
using Backend.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace Backend.Services.Auth;

public class AuthService : IAuthService
{
  private readonly IUserRepository _userRepo;
  private readonly IConfiguration _config;
  private readonly AppDbContext _context;
  private readonly IMapper _mapper;

  public AuthService(IUserRepository userRepo, IConfiguration config, AppDbContext context, IMapper mapper)
  {
    _userRepo = userRepo;
    _config = config;
    _context = context;
    _mapper = mapper;
  }

  public async Task<AuthResponseDto?> Login(LoginRequestDto dto)
  {
    var user = await _context.Users
        .Include(u => u.Role)
        .Include(u => u.Candidate)
        .Include(u => u.Employee)
        .FirstOrDefaultAsync(u => u.Email == dto.Email);

    if (user == null) return null;
    if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password)) return null;

    return GenerateToken(user);
  }

  public async Task<AuthResponseDto> Register(RegisterRequestDto dto)
  {
    var existing = await _context.Users.AnyAsync(u => u.Email == dto.Email);
    if (existing) throw new Exception("Email already used");

    var user = _mapper.Map<User>(dto);
    user.Id = Guid.NewGuid();

    await _userRepo.Add(user);

    // Load the user with relations for token generation
    var userWithRelations = await _context.Users
        .Include(u => u.Role)
        .Include(u => u.Candidate)
        .Include(u => u.Employee)
        .FirstAsync(u => u.Id == user.Id);

    return GenerateToken(userWithRelations);
  }

  private AuthResponseDto GenerateToken(User user)
  {
    var key = _config["Jwt:Key"] ?? throw new Exception("Jwt:Key missing");

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
