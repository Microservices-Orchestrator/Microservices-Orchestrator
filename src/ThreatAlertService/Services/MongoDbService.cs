using MongoDB.Driver;
using ThreatAlertService.Models;

namespace ThreatAlertService.Services;

public class MongoDbService
{
    private readonly IMongoCollection<ThreatAlert> _threatAlertsCollection;

    public MongoDbService(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MongoDb");
        var mongoClient = new MongoClient(connectionString);
        var mongoDatabase = mongoClient.GetDatabase("ThreatAlertDb");

        _threatAlertsCollection = mongoDatabase.GetCollection<ThreatAlert>("Threats");
    }

    public async Task CreateAsync(ThreatAlert newAlert) =>
        await _threatAlertsCollection.InsertOneAsync(newAlert);
}