using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AuthLogService.Models;

public class AuthLog
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string Username { get; set; } = null!;
    
    public string IpAddress { get; set; } = null!;
    
    public bool IsSuccess { get; set; }
    
    public DateTime AttemptDate { get; set; } = DateTime.UtcNow;
}