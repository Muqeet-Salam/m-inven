using authapi.Data;
using authapi.DTOs;
using authapi.Models;
using MongoDB.Driver;

namespace authapi.Services;

public class InventoryService : IInventoryService {
    private readonly MongoDbContext _mongoDb;

    public InventoryService(MongoDbContext mongoDb) {
        _mongoDb = mongoDb;
    }

    public async Task<bool> StockInAsync(InventoryTransactionDTO dto, string userId) {
        var product = await _mongoDb.Products
            .Find(p => p.Id == dto.ProductId)
            .FirstOrDefaultAsync();
        
        if (product == null) return false;

        product.AddStock(dto.Quantity);

        await _mongoDb.Products.ReplaceOneAsync(
            p => p.Id == dto.ProductId,
            product
        );

        var transaction = new InventoryTransaction (
            dto.ProductId,
            "StockIn",
            dto.Quantity,
            userId,
            dto.Reason);

        await _mongoDb.InventoryTransactions.InsertOneAsync(transaction);
        return true;
    }

    public async Task<bool> StockOutAsync(InventoryTransactionDTO dto, string userId) {
        var product = await _mongoDb.Products
            .Find(p => p.Id == dto.ProductId)
            .FirstOrDefaultAsync();
        
        if (product == null) return false;

        if (product.StockQuantity < dto.Quantity) return false;

        product.RemoveStock(dto.Quantity);

        await _mongoDb.Products.ReplaceOneAsync(
            p => p.Id == dto.ProductId,
            product
        );

        var transaction = new InventoryTransaction(
            dto.ProductId,
            "StockOut",
            dto.Quantity,
            userId,
            dto.Reason);

        await _mongoDb.InventoryTransactions.InsertOneAsync(transaction);

        return true;
    }
}