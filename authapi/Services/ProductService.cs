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
    public async Task<List<Product>> GetAllAsync() {
        return await _mongoDb.Products
            .Find(_ => true)
            .ToListAsync();
    }
}