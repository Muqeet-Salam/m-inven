using authapi.Data;
using authapi.Models;
using MongoDB.Driver;

namespace authapi.Services;

public class InventoryService {
    private readonly MongoDbContext _mongoDb;
    
    public InventoryService(MongoDbContext mongoDb) {
        _mongoDb = mongoDb;
    }

    public async Task<bool> StockInAsync(
        string productId,
        int quantity,
        string? reason,
        string userId) {
        var product = await _mongoDb.Products
            .Find(p => p.Id == productId)
            .FirstOrDefaultAsync();
        
        if (product == null) return false;

        product.StockQuantity += quantity;
        product.UpdatedAt = DateTime.UtcNow;

        await _mongoDb.Products.ReplaceOneAsync(
            p => p.Id == productId,
            product
        );

        var transaction = new InventoryTransaction {
            ProductId = productId,
            Type = "StockIn",
            Quantity = quantity,
            Reason = reason,
            UserId = userId
        };

        await _mongoDb.InventoryTransactions.InsertOneAsync(transaction);
        return true;
    }

    public async Task<bool> StockOutAsync(
        string productId,
        int quantity,
        string? reason,
        string userId) {
        var product = await _mongoDb.Products
            .Find(p => p.Id == productId)
            .FirstOrDefaultAsync();
        
        if (product == null) return false;

        if (product.StockQuantity < quantity) return false;

        product.StockQuantity -= quantity;
        product.UpdatedAt = DateTime.UtcNow;

        await _mongoDb.Products.ReplaceOneAsync(
            p => p.Id == productId,
            product
        );

        var transaction = new InventoryTransaction {
            ProductId = productId,
            Type = "StockOut",
            Quantity = quantity,
            Reason = reason,
            UserId = userId
        };

        await _mongoDb.InventoryTransactions.InsertOneAsync(transaction);

        return true;
    }
}