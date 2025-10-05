using Backend.Dtos.Auth;

namespace Backend.Services.Interfaces;

public interface IAuthService
{
  Task<AuthResponseDto?> Login(LoginRequestDto dto);
  Task<AuthResponseDto> Register(RegisterRequestDto dto);
}
