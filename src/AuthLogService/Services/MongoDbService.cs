using MongoDB.Driver;
using AuthLogService.Models;

namespace AuthLogService.Services;

public class MongoDbService
{
    private readonly IMongoCollection<AuthLog> _authLogs;

    public MongoDbService(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MongoDb");
        var mongoClient = new MongoClient(connectionString);
        var mongoDatabase = mongoClient.GetDatabase("AuthLogDb"); // Veritabanı adı

        _authLogs = mongoDatabase.GetCollection<AuthLog>("LoginAttempts"); // Tablo (Koleksiyon) adı
    }

    public async Task LogLoginAttemptAsync(string username, string ipAddress, bool isSuccess)
    {
        var log = new AuthLog
        {
            Username = username,
            IpAddress = ipAddress,
            IsSuccess = isSuccess,
            AttemptDate = DateTime.UtcNow
        };

        await _authLogs.InsertOneAsync(log);
    }
}