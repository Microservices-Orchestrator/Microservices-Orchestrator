using Microsoft.AspNetCore.Http;

namespace DispatcherGateway.Tests
{
    internal class JwtAuthMiddleware
    {
        private RequestDelegate next;

        public JwtAuthMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        internal async Task InvokeAsync(DefaultHttpContext context)
        {
            throw new NotImplementedException();
        }
    }
}