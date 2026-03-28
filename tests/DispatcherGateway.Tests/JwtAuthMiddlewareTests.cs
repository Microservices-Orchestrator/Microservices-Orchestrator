using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DispatcherGateway.Tests
{
    public class JwtAuthMiddlewareTests
    {
        [Fact]
        public async Task InvokeAsync_NoToken_Returns401()
        {
            var context = new DefaultHttpContext();
            RequestDelegate next = (HttpContext ctx) => Task.CompletedTask;

            var middleware = new JwtAuthMiddleware(next);

            await middleware.InvokeAsync(context);

            Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);

        }
    }
}
