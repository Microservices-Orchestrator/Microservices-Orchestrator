using Microsoft.AspNetCore.Http;
using System.Text;

namespace DispatcherGateway
{
    public class RouterService
    {
        private readonly HttpClient _httpClient;
        private readonly Dictionary<string, string> _routes;

        public RouterService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _routes = new Dictionary<string, string>();
        }

        public void AddRoute(string path, string targetUrl)
        {
            _routes[path] = targetUrl;
        }


        public async Task<HttpResponseMessage> ForwardRequestAsync(HttpContext context)
        {
            var path = context.Request.Path.Value;
            var method = context.Request.Method;

            if (string.IsNullOrEmpty(path))
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            }

            var route = _routes.FirstOrDefault(r => path.StartsWith(r.Key));

            if (route.Key != null)
            {
                var targetUrl = route.Value;
                var request = new HttpRequestMessage(new HttpMethod(method), targetUrl + path);


                if (context.Request.ContentLength > 0)
                {
                    // İstek gövdesini okuyoruz
                    context.Request.EnableBuffering();
                    var streamContent = new StreamContent(context.Request.Body);


                    streamContent.Headers.ContentType =
                        new System.Net.Http.Headers.MediaTypeHeaderValue(context.Request.ContentType ?? "application/json");

                    request.Content = streamContent;
                }


                foreach (var header in context.Request.Headers)
                {

                    if (header.Key.ToLower() == "content-type") continue;

                    if (!request.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()))
                    {
                        request.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                    }
                }

                return await _httpClient.SendAsync(request);
            }

            return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
        }
    }
}