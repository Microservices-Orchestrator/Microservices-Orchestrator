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
        public async Task<HttpResponseMessage> ForwardRequestAsync(string? path, string method)
        { 
             if(string.IsNullOrEmpty(path))
             {
                 return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest); 
             }
            var route = _routes.FirstOrDefault(r => path.StartsWith(r.Key));

            if (route.Key != null) { 
                var targetUrl = route.Value;

                var request = new HttpRequestMessage(new HttpMethod(method), targetUrl + path);

                return await _httpClient.SendAsync(request);
            }
            else
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
            }
        }
    }
}
