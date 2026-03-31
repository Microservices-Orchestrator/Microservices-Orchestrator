using MongoDB.Driver;
using AuthLogService.Models;

namespace AuthLogService.Services;

public class MongoDbService
{
    private readonly IMongoCollection<AuthLog> _authLogs;
    private readonly IMongoCollection<User> _usersCollection;

    public MongoDbService(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MongoDb");
        var mongoClient = new MongoClient(connectionString);
        var mongoDatabase = mongoClient.GetDatabase("AuthLogDb"); // Veritabanı adı
 
        _authLogs = mongoDatabase.GetCollection<AuthLog>("LoginAttempts"); // Tablo (Koleksiyon) adı
        _usersCollection = mongoDatabase.GetCollection<User>("Users");
    }

    public async Task<User?> GetUserByUsernameAsync(string username) =>
        await _usersCollection.Find(u => u.Username == username).FirstOrDefaultAsync();

    public async Task CreateUserAsync(User newUser) =>
        await _usersCollection.InsertOneAsync(newUser);

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