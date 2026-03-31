using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using AuthLogService.Services;
using BCrypt.Net;
using AuthLogService.Models;

namespace AuthLogService.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly MongoDbService _mongoDbService;

    public AuthController(MongoDbService mongoDbService)
    {
        _mongoDbService = mongoDbService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var existingUser = await _mongoDbService.GetUserByUsernameAsync(request.Username);
        if (existingUser != null)
        {
            return Conflict(new { Message = "Bu kullanıcı adı zaten mevcut." });
        }

        var newUser = new User
        {
            Username = request.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        await _mongoDbService.CreateUserAsync(newUser);

        return CreatedAtAction(nameof(Register), new { id = newUser.Id }, new { Message = "Kullanıcı başarıyla oluşturuldu." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // 1. İsteği yapanın IP adresini al
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Bilinmiyor";

        // 2. Kullanıcıyı veritabanından bul ve şifreyi doğrula
        var user = await _mongoDbService.GetUserByUsernameAsync(request.Username);

        bool isSuccess = (user != null && BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash));

        // 3. MongoDB'ye log yazma işlemi (IP adresi, denenen kullanıcı adı, başarılı/başarısız durumu)
        await _mongoDbService.LogLoginAttemptAsync(request.Username, ipAddress, isSuccess);

        if (isSuccess)
        {
            // TODO: 4. Başarılı ise JWT üret ve geri dön
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("BenimCokGizliVeGuvenliAnahtarim12345!"); // Program.cs ile aynı şifre olmalı!
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, request.Username) }),
                Expires = DateTime.UtcNow.AddHours(1), // Token 1 saat geçerli olacak
                Issuer = "http://localhost:5064",
                Audience = "http://localhost:5064",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtString = tokenHandler.WriteToken(token);

            return Ok(new { Message = "Giriş başarılı", Token = jwtString });
        }
        else
        {
            return Unauthorized(new { Message = "Giriş başarısız. Kullanıcı adı veya şifre hatalı." });
        }
    }
}

// Kullanıcıdan gelen JSON isteğini temsil eden model
public record LoginRequest(string Username, string Password);
public record RegisterRequest(string Username, string Password);