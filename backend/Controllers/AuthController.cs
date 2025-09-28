using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using recruitem_backend.Data;
using recruitem_backend.Models;
using recruitem_backend.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace recruitem_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IPasswordService _passwordService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(DatabaseContext context, IPasswordService passwordService, ILogger<AuthController> logger)
        {
            _context = context;
            _passwordService = passwordService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Please fill all required fields correctly");
                }

                _logger.LogInformation($"New user trying to register with email: {request.Email}");

                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (existingUser != null)
                {
                    return BadRequest("User with this email already exists");
                }

                var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == request.RoleName);
                if (role == null)
                {
                    return BadRequest("Invalid role selected");
                }

                var hashedPassword = _passwordService.HashPassword(request.Password);

                var newUser = new User();
                newUser.Id = Guid.NewGuid();
                newUser.Email = request.Email;
                newUser.Password = hashedPassword;
                newUser.RoleId = role.Id;
                newUser.CreatedAt = DateTime.UtcNow;
                newUser.UpdatedAt = DateTime.UtcNow;

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"User registered successfully: {request.Email}");

                var token = GenerateJwtToken(newUser);

                return Ok(new
                {
                    message = "Registration successful",
                    token = token,
                    user = new
                    {
                        id = newUser.Id,
                        email = newUser.Email,
                        role = role.RoleName
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during registration: {ex.Message}");
                return BadRequest("Registration failed. Please try again.");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                {
                    return BadRequest("Email and password are required");
                }

                _logger.LogInformation($"User trying to login: {request.Email}");

                var user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email == request.Email);

                if (user == null)
                {
                    _logger.LogWarning($"Login failed: No user found with email {request.Email}");
                    return BadRequest("Invalid email or password");
                }

                bool passwordIsCorrect = _passwordService.VerifyPassword(request.Password, user.Password);
                if (!passwordIsCorrect)
                {
                    _logger.LogWarning($"Login failed: Wrong password for {request.Email}");
                    return BadRequest("Invalid email or password");
                }

                _logger.LogInformation($"User logged in successfully: {request.Email}");

                var token = GenerateJwtToken(user);

                return Ok(new
                {
                    message = "Login successful",
                    token = token,
                    user = new
                    {
                        id = user.Id,
                        email = user.Email,
                        role = user.Role.RoleName
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during login: {ex.Message}");
                return BadRequest("Login failed. Please try again.");
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email))
                {
                    return BadRequest("Email is required");
                }

                _logger.LogInformation($"Password reset requested for: {request.Email}");

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (user == null)
                {
                    return Ok("If the email exists, a reset link has been sent");
                }

                var resetToken = Guid.NewGuid().ToString();
                _logger.LogInformation($"Reset token generated for: {request.Email}");

                return Ok("If the email exists, a reset link has been sent");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in forgot password: {ex.Message}");
                return BadRequest("Something went wrong. Please try again.");
            }
        }        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password) || string.IsNullOrEmpty(request.Token))
                {
                    return BadRequest("All fields are required");
                }

                _logger.LogInformation($"Password reset attempt for: {request.Email}");

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (user == null)
                {
                    return BadRequest("Invalid reset request");
                }

                var newHashedPassword = _passwordService.HashPassword(request.Password);

                user.Password = newHashedPassword;
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Password reset successful for: {request.Email}");
                return Ok("Password has been reset successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error resetting password: {ex.Message}");
                return BadRequest("Password reset failed. Please try again.");
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            _logger.LogInformation("User logging out");
            return Ok("Logout successful");
        }

        private string GenerateJwtToken(User user)
        {
            _logger.LogInformation($"Creating token for user: {user.Email}");

            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = "YourSuperSecretKeyThatIsAtLeast32CharactersLongForMaximumSecurity!";
            var key = Encoding.UTF8.GetBytes(secretKey);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            _logger.LogInformation($"Token created successfully for user: {user.Email}");
            return tokenString;
        }
    }

    public class RegisterRequest
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string RoleName { get; set; } = "";
    }

    public class LoginRequest
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class ForgotPasswordRequest
    {
        public string Email { get; set; } = "";
    }

    public class ResetPasswordRequest
    {
        public string Email { get; set; } = "";
        public string Token { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
