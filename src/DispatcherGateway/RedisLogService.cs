
using Microsoft.AspNetCore.Connections;
using StackExchange.Redis;

namespace DispatcherGateway
{
    public class RedisLogService : ILogService
    {
        private readonly ConnectionMultiplexer _redis;

        public RedisLogService(IConfiguration configuration)
        {
            string redisConnectionString = configuration.GetConnectionString("Redis");
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
                await db.ListRightPushAsync("request_logs", $"Path: {context.Request.Path}, Method: {context.Request.Method}, IP: {ipAdresi}");
            }
            catch(Exception e)
            {
                Console.WriteLine($"[UYARI] Redis loglanamadi. Hata: {e.Message}");
            }
            

        }

      
    }
}
