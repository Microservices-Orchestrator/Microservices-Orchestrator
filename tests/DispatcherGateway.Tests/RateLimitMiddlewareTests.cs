using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DispatcherGateway.Tests
{
    public  class RateLimitMiddlewareTests
    {
        [Fact]
        public async Task TestRateLimitMiddleware()
        {
            var context = new DefaultHttpContext();
            RequestDelegate requestDelegate = (HttpContext ctx) =>
            {
                return Task.CompletedTask;
            };
            RateLimitMiddleware middleware = new RateLimitMiddleware(requestDelegate);
            for (int i = 0; i < 6; i++)
            {
                await middleware.InvokeAsync(context);
            }
            Assert.Equal(StatusCodes.Status429TooManyRequests, context.Response.StatusCode);
        }
    }
}
