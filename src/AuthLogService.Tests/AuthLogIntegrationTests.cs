using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace AuthLogService.Tests;

public class AuthLogIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AuthLogIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Login_WithInvalidData_ReturnsBadRequest_400()
    {
        // Arrange
        var client = _factory.CreateClient();
        
        // AuthController'daki "Kullanıcı adı ve şifre boş olamaz" kontrolüne (400) takılması için boş obje yolluyoruz.
        var invalidRequest = new { }; 

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", invalidRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_WithInvalidData_ReturnsBadRequest_400()
    {
        // Arrange
        var client = _factory.CreateClient();

        // AuthController'daki register metodunun 400 Bad Request kontrolüne takılması için boş obje yolluyoruz.
        var invalidRequest = new { };

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/register", invalidRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_WithValidData_ReturnsCreated_201()
    {
        // Arrange
        var client = _factory.CreateClient();
        
        // Veritabanında çakışma olmaması için her testte benzersiz bir kullanıcı adı kullanıyoruz.
        var uniqueUsername = $"testuser_{Guid.NewGuid()}";
        var validRequest = new { Username = uniqueUsername, Password = "password123" };

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/register", validRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOk_200()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Önce test için benzersiz bir kullanıcı oluşturalım
        var uniqueUsername = $"testuser_{Guid.NewGuid()}";
        var credentials = new { Username = uniqueUsername, Password = "password123" };

        // Kullanıcıyı kaydet ve kaydın başarılı olduğundan emin ol
        var registerResponse = await client.PostAsJsonAsync("/api/auth/register", credentials);
        registerResponse.EnsureSuccessStatusCode(); 

        // Act
        // Şimdi aynı bilgilerle giriş yapmayı dene
        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", credentials);

        // Assert
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
    }
}