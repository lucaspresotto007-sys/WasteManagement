using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WasteManagementAPI.Data;
using WasteManagementAPI.Models;
using WasteManagementAPI.ViewModels;

namespace WasteManagementAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<UserTokenViewModel?> RegisterAsync(RegisterViewModel model)
        {
            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                return null; // Email already in use
            }

            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                PasswordHash = HashPassword(model.Password),
                Role = string.IsNullOrWhiteSpace(model.Role) ? "Resident" : model.Role,
                RecyclingPoints = 0
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserTokenViewModel
            {
                Token = GenerateJwtToken(user),
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                RecyclingPoints = user.RecyclingPoints
            };
        }

        public async Task<UserTokenViewModel?> LoginAsync(LoginViewModel model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null || !VerifyPassword(model.Password, user.PasswordHash))
            {
                return null; // Invalid credentials
            }

            return new UserTokenViewModel
            {
                Token = GenerateJwtToken(user),
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                RecyclingPoints = user.RecyclingPoints
            };
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtKey = _configuration["Jwt:Key"] ?? "WasteManagementAPI_Secret_Security_Key_2026_FIAP";
            var key = Encoding.ASCII.GetBytes(jwtKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"] ?? "WasteManagementAPI",
                Audience = _configuration["Jwt:Audience"] ?? "WasteManagementAPI"
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            return HashPassword(password) == passwordHash;
        }
    }
}
