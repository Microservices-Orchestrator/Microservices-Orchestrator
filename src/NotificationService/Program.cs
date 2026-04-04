using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

var builder = WebApplication.CreateBuilder(args);

// İZOLASYON SAĞLANDI: Sadece bu servise ait yeni bir veritabanı (NotificationDb)
var mongoConnectionString = Environment.GetEnvironmentVariable("MongoDbSettings__ConnectionString") ?? "mongodb://localhost:27017";
var client = new MongoClient(mongoConnectionString);
var db = client.GetDatabase("NotificationDb");
var collection = db.GetCollection<Notification>("Notifications");

var app = builder.Build();

// YENİ ALARM EKLEME (POST)
app.MapPost("/api/notifications", async (Notification notif) =>
{
    notif.Id = ObjectId.GenerateNewId().ToString();
    notif.CreatedAt = DateTime.UtcNow;
    await collection.InsertOneAsync(notif);
    return Results.Created($"/api/notifications/{notif.Id}", notif);
});

// ALARMLARI LİSTELEME (GET)
app.MapGet("/api/notifications", async () =>
{
    // En son gelen 50 alarmı listele
    var list = await collection.Find(_ => true).SortByDescending(n => n.CreatedAt).Limit(50).ToListAsync();
    return Results.Ok(list);
});

app.Run();

// MODEL
public class Notification
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Level { get; set; } = "Info"; // Seçenekler: Info, Warning, Critical
    public DateTime CreatedAt { get; set; }
}