using DispatcherGateway;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<RouterService>(); // RouterService için HttpClient ekliyoruz


var app = builder.Build();

app.UseMiddleware<JwtAuthMiddleware>(); // JWT doðrulama middleware'ini ekliyoruz


app.Map("/{**catch-all}", async (HttpContext context, RouterService routerService) =>
{
    routerService.AddRoute("/api/users", "https://jsonplaceholder.typicode.com");
    var request = context.Request.Path.Value; // Ýstek yolunu al
    var response = await routerService.ForwardRequestAsync(request); // Ýsteði yönlendir

    if (response != null)
    {  
        var content = await response.Content.ReadAsStringAsync(); // Yanýt içeriðini oku
        return Results.Content(content, "application/json"); // Yanýtý döndür
    }
    else
    {
        return Results.NotFound("Router Bulunamadý");
    }

});

app.Run();

