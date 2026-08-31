using authapi.Data;
using authapi.Models;
using MongoDB.Driver;

namespace authapi.Services;

public class ProductService {
    private readonly MongoDbContext _mongoDb;
    
    public ProductService(MongoDbContext mongoDb) {
        _mongoDb = mongoDb;
    }

    public async Task<Product> CreateAsync(Product product) {
        await _mongoDb.Products.InsertOneAsync(product);
        return product;
    }
    public async Task<Product?> GetByNameAsync(string name) {
    return await _mongoDb.Products
        .Find(product => product.Name == name)
        .FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateAsync(string name, Product product) {
        product.Name = name;
        product.UpdatedAt = DateTime.UtcNow;

        var result = await _mongoDb.Products.ReplaceOneAsync(existing => existing.Name == name, product);
        return result.MatchedCount > 0;
    }

    public async Task<bool> DeleteAsync(string name, Product product) {
        var result = await _mongoDb.Products.DeleteOneAsync(product => product.Name == name);

        return result.DeletedCount > 0;
    }
    
    public async Task<List<Product>> GetAllAsync() {
        return await _mongoDb.Products
            .Find(_ => true)
            .ToListAsync();
    }

}