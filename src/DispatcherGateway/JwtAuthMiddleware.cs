using DispatcherGateway;

namespace DispatcherGateway
{
    public class JwtAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path;
            if (path.StartsWithSegments("/health") ||
                path.StartsWithSegments("/api/auth/login") ||
                path.StartsWithSegments("/api/auth/register"))
            {
                await _next(context);
                return;
            }
            if (!context.Request.Headers.ContainsKey("Authorization"))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }
            else
            {
                await _next(context);
            }

        }
    }
}
