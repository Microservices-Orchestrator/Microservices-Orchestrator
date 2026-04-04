using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ThreatAlertService.Models;

public class ThreatAlert
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string SourceIp { get; set; } = null!;

    public string ThreatType { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public bool IsCritical { get; set; } = false;
}