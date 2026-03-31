
using Microsoft.AspNetCore.Connections;
using StackExchange.Redis;

namespace DispatcherGateway
{
    public class RedisLogService : ILogService
    {
        private readonly ConnectionMultiplexer _redis;

        public RedisLogService()
        {
            _redis = ConnectionMultiplexer.Connect("localhost:6379");
        }
        public Task LogRequest(HttpContext context)
        {
            var db = _redis.GetDatabase();

            string ipAdresi;

            if(context.Connection.RemoteIpAddress != null)
            {
                ipAdresi = context.Connection.RemoteIpAddress.ToString();
            }
            else
            {
                ipAdresi = "IP Adresi Bilinmiyor";
            }
            return db.ListRightPushAsync("request_logs", $"Path: {context.Request.Path}, Method: {context.Request.Method}, IP: {ipAdresi}");

        }

      
    }
}
