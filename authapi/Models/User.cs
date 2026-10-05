using authapi.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace authapi.Models;

public class User {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    [BsonRepresentation(BsonType.String)]
    public Role Role { get; set; } = Role.Viewer;
}