using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DispatcherGateway;

namespace DispatcherGateway.Tests
{
    public class RequestLogMiddlewareTests
    {
        [Fact]

        public async Task InvokeAsyncLogMethod()
        {
            var context = new DefaultHttpContext();
                context.Request.Path = "/api/tehdit";

            RequestDelegate next = (HttpContext ctx) => Task.CompletedTask;

            var mockLogService = new Mock<ILogService>();

            var middleware = new RequestLogMiddleware(next,mockLogService.Object);
                await middleware.InvokeAsync(context);
    
                mockLogService.Verify(x => x.LogRequest(It.IsAny<HttpContext>()), Times.Once);
        }
    }
}
