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
}