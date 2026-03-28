using Microsoft.AspNetCore.Mvc;

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
            string fakeToken = "ornek_jwt_token_buraya_gelecek"; 
            
            return Ok(new { Message = "Giriş başarılı", Token = fakeToken });
        }
        else
        {
            return Unauthorized(new { Message = "Giriş başarısız. Kullanıcı adı veya şifre hatalı." });
        }
    }
}

// Kullanıcıdan gelen JSON isteğini temsil eden model
public record LoginRequest(string Username, string Password);