
using Microsoft.AspNetCore.Connections;
using StackExchange.Redis;
using System.Net;
using System.Text.Json;

namespace DispatcherGateway
{
    public class RedisLogService : ILogService
    {
        private readonly ConnectionMultiplexer _redis;

        public RedisLogService(IConfiguration configuration)
        {
            string? redisConnectionString = configuration.GetConnectionString("Redis");
            if (string.IsNullOrWhiteSpace(redisConnectionString))
            {
                throw new ArgumentNullException(nameof(redisConnectionString), "Redis bağlantı adresi appsettings.json veya çevre değişkenleri içinde bulunamadı!");
            }
            _redis = ConnectionMultiplexer.Connect(redisConnectionString);
        }
        public async Task LogRequest(HttpContext context)
        {
            try
            {
                var db = _redis.GetDatabase();

                string ipAdresi;

                if (context.Connection.RemoteIpAddress != null)
                {
                    ipAdresi = context.Connection.RemoteIpAddress.ToString();
                }
                else
                {
                    ipAdresi = "IP Adresi Bilinmiyor";
                }
                var logData = new
                {
                    IPAddress = ipAdresi,
                    Path = context.Request.Path.ToString(),
                    Method = context.Request.Method,
                    Timestamp = DateTime.UtcNow
                };
                string jsonLog = JsonSerializer.Serialize(logData);
                await db.ListRightPushAsync("request_logs", jsonLog);
            }
            catch(Exception e)
            {
                Console.WriteLine($"[UYARI] Redis loglanamadi. Hata: {e.Message}");
            }
            

        }

      
    }
}
