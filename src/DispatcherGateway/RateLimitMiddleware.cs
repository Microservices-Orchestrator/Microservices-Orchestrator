namespace DispatcherGateway
{
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private static Dictionary<string, (int Count, DateTime Timestamp)> _requestCounts = new Dictionary<string, (int, DateTime)>();
        private static readonly object _lock = new object();
        private const int LIMIT = 5;
        private static readonly TimeSpan TIME_WINDOW = TimeSpan.FromMinutes(1);
        public RateLimitMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            string ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            bool limit = false;
            lock (_lock)
            {
                if (_requestCounts.ContainsKey(ipAddress))
                {
                    var (count, timestamp) = _requestCounts[ipAddress];
                    if (DateTime.UtcNow - timestamp < TIME_WINDOW)
                    {
                        if (count >= LIMIT)
                        {
                            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                            limit = true;
                        }
                        else
                        {
                            _requestCounts[ipAddress] = (count + 1, timestamp);
                        }
                    }
                    else
                    {
                        _requestCounts[ipAddress] = (1, DateTime.UtcNow);
                    }
                }
                else
                {
                    _requestCounts[ipAddress] = (1, DateTime.UtcNow);
                }
            }
            if (limit)
            {
               
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync("{\"statusCode\": 429, \"error\": \"Too Many Requests\", \"message\": \"Rate limit exceeded. Please try again in one minute.\"}");
                return;
            }
                await _next(context);
        }
    }
}
