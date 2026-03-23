using Moq;
using Moq.Protected;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DispatcherGateway.Tests
{
    public class DispatcherRoutingTests
    {
        [Fact]
        public async Task TestDispatcherRouting() // Düzeltme 1: async Task eklendi
        {
            
            var handlerMock = new Mock<HttpMessageHandler>();

           
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{ \"status\": \"success\" }")
                });

            var httpClient = new HttpClient(handlerMock.Object);
            httpClient.BaseAddress = new Uri("http://localhost:5000"); // Gateway'in kendi adresi

           
            var routerService = new RouterService(httpClient);

           
            routerService.AddRoute("/api/users", "http://localhost:5001");

            
            var response = await routerService.ForwardRequestAsync("/api/users");

            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

           
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri == new Uri("http://localhost:5001/api/users")
                ),
                ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}