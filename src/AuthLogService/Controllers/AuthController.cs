using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AuthLogService.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // 1. İsteği yapanın IP adresini al
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Bilinmiyor";

        // 2. Basit bir doğrulama simülasyonu (İleride gerçek bir veritabanından yapılabilir)
        bool isSuccess = (request.Username == "admin" && request.Password == "123456");

        // TODO: 3. MongoDB'ye log yazma işlemi (IP adresi, denenen kullanıcı adı, başarılı/başarısız durumu, tarih)
        // _mongoDbService.LogLoginAttempt(request.Username, ipAddress, isSuccess);

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