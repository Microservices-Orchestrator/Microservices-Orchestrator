namespace DispatcherGateway
{
    public class RouterService
    {
        private readonly HttpClient _httpClient; // İnternete çıkıp istek atacak postacımız

        private readonly Dictionary<string, string> _routes;     // Route'ları tutacağımız sözlük

        public RouterService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _routes = new Dictionary<string, string>();

        }
        public void AddRoute(string path, string targetUrl)
        {
            _routes[path] = targetUrl; // Route'u ekle veya güncelle
        }
        public async Task<HttpResponseMessage> ForwardRequestAsync(string path)
        { 
             if (_routes.TryGetValue(path, out var targetUrl))
             {
                var request = new HttpRequestMessage(HttpMethod.Get, targetUrl + path); // İstek oluştur
                 return await _httpClient.SendAsync(request); // İsteği gönder ve sonucu döndür
             }
            else
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound); // Route bulunamazsa 404 döndür
            }
        }
    }
}
