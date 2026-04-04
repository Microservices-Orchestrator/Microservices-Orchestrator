namespace DispatcherGateway
{
    public class RequestLogMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogService _logService;
        public RequestLogMiddleware(RequestDelegate next, ILogService logService)
        {
            _next = next;
            _logService = logService;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            await _logService.LogRequest(context);
            await _next(context); 
        }
    }
}
