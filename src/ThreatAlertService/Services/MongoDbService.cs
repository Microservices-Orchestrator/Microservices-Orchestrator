using MongoDB.Driver;
using System.Collections.Generic;
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

    public async Task<List<ThreatAlert>> GetAsync() =>
        await _threatAlertsCollection.Find(_ => true).ToListAsync();

    public async Task<ThreatAlert?> GetAsync(string id) =>
        await _threatAlertsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
}