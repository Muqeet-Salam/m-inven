using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace authapi.Models;

public class InventoryTransaction {
    
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string ProductId { get; set; } = null!;
    public string Type { get; set; } = null!;
    public int Quantity { get; set; }
    public string? Reason { get; set; }
    public string UserId { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    private InventoryTransaction() {}

    public InventoryTransaction(
        string productId,
        string type, 
        int quantity,
        string userId,
        string? reason = null) {
        if (string.IsNullOrWhiteSpace(productId)) {
            throw new ArgumentException("Product Id is required.");   
        };
        
        if (string.IsNullOrWhiteSpace(userId)) {
            throw new ArgumentException("UserId is required.");   
        };

        if (type != "StockIn" && type != "StockOut") {
            throw new ArgumentException("Invalid Transaction Type.");
        };

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");
    
    ProductId = productId;
    Type = type;
    Quantity = quantity;
    Reason = reason;
    UserId = userId;
    CreatedAt = DateTime.UtcNow;
    }
}