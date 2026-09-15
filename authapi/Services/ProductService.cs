using authapi.Data;
using authapi.DTOs;
using authapi.Models;
using MongoDB.Driver;

namespace authapi.Services;

public class ProductService {
    private readonly MongoDbContext _mongoDb;
    
    public ProductService(MongoDbContext mongoDb) {
        _mongoDb = mongoDb;
    }

    public async Task<ProductResponse> CreateAsync(ProductCreate dto) {
        var product = new Product {
          Name = dto.Name,
          SKU = dto.SKU,
          Description = dto.Description,
          Category = dto.Category,
          Price = dto.Price,
          StockQuantity = dto.StockQuantity,
          MinimumStock = dto.MinimumStock,
          CreatedAt = DateTime.UtcNow,
          UpdatedAt = DateTime.UtcNow
        };

        await _mongoDb.Products.InsertOneAsync(product);
        return ToResponse(product);
    }

    public async Task<ProductResponse?> GetByNameAsync(string name) {
        var product = await _mongoDb.Products
            .Find(product => product.Name == name)
            .FirstOrDefaultAsync();
        if (product == null) return null;

        return ToResponse(product);
    }

    public async Task<List<ProductResponse>> GetAllAsync() {
        var products = await _mongoDb.Products
            .Find(_ => true)
            .ToListAsync();
        
        return products
            .Select(ToResponse)
            .ToList();
    }

    public async Task<bool> UpdateAsync(string name, ProductUpdate dto) {
        var existingProduct = await _mongoDb.Products
            .Find(product => product.Name == name)
            .FirstOrDefaultAsync();

        if (existingProduct == null) return false;

        existingProduct.SKU = dto.SKU;
        existingProduct.Description = dto.Description;
        existingProduct.Category = dto.Category;
        existingProduct.Price = dto.Price;
        existingProduct.MinimumStock = dto.MinimumStock;
        existingProduct.UpdatedAt = DateTime.UtcNow;

        var result = await _mongoDb.Products.ReplaceOneAsync(
            product => product.Name == name,
            existingProduct
        );
        
        return result.MatchedCount > 0;
    }

    public async Task<bool> DeleteAsync(string name) {
        var result = await _mongoDb.Products.DeleteOneAsync(
            product => product.Name == name
        );

        return result.DeletedCount > 0;
    }

    private static ProductResponse ToResponse(Product product) {
        return new ProductResponse {
            Id = product.Id,
            Name = product.Name,
            SKU = product.SKU,
            Description = product.Description,
            Category = product.Category,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            MinimumStock = product.MinimumStock,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

}