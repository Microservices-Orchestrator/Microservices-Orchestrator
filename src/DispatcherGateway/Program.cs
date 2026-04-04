using DispatcherGateway;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<RouterService>(); // RouterService i�in HttpClient ekliyoruz
builder.Services.AddSingleton<ILogService, RedisLogService>(); // ILogService i�in RedisLogService ekliyoruz
builder.Services.AddHealthChecks()
    .AddRedis(builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379", name: "Redis Check");
var app = builder.Build();

app.UseMiddleware<RequestLogMiddleware>();
app.UseMiddleware<RateLimitMiddleware>();
app.UseMiddleware<JwtAuthMiddleware>(); // JWT do�rulama middleware'ini ekliyoruz
app.MapHealthChecks("/health"); // Sa�l�k kontrol� endpoint'i ekliyoruz

app.Map("/{**catch-all}", async (HttpContext context, RouterService routerService) =>
{
    routerService.AddRoute("/api/threats", "http://threat-service");
    routerService.AddRoute("/api/auth", "http://auth-service");
    routerService.AddRoute("/api/notifications", "http://notification-service:8080");
    routerService.AddRoute("/api/users", "https://jsonplaceholder.typicode.com");

    var request = context.Request.Path.Value; // �stek yolunu al
    var response = await routerService.ForwardRequestAsync(context); // RouterService ile istei ilet

    if (response != null)
    {
        var content = await response.Content.ReadAsStringAsync(); // Yan�t i�eri�ini oku
        return Results.Content(content, "application/json", statusCode: (int)response.StatusCode);
    }
    else
    {
        return Results.NotFound("Router Bulunamad�");
    }

});

app.Run();

