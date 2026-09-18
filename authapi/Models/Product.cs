using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace authapi.Models;

public class Product
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; private set; }

    public string Name { get; private set; } = null!;

    public string SKU { get; private set; } = null!;

    public string Description { get; private set; } = null!;

    public string Category { get; private set; } = null!;

    public decimal Price { get; private set; }

    public int StockQuantity { get; private set; }

    public int MinimumStock { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    private Product()
    {
    }

    public Product(
        string name,
        string sku,
        string description,
        string category,
        decimal price,
        int stockQuantity,
        int minimumStock) {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name is required.");

        if (string.IsNullOrWhiteSpace(sku))
            throw new ArgumentException("SKU is required.");

        if (price < 0)
            throw new ArgumentException("Price cannot be negative.");

        if (stockQuantity < 0)
            throw new ArgumentException("Stock quantity cannot be negative.");

        if (minimumStock < 0)
            throw new ArgumentException("Minimum stock cannot be negative.");

        Name = name;
        SKU = sku;
        Description = description;
        Category = category;
        Price = price;
        StockQuantity = stockQuantity;
        MinimumStock = minimumStock;

        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(
        string name,
        string sku,
        string description,
        string category,
        decimal price,
        int minimumStock) {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product Name is required.");

        if (string.IsNullOrWhiteSpace(sku))
            throw new ArgumentException("SKU is required.");

        if (price < 0)
            throw new ArgumentException("Price cannot be negative.");

        if (minimumStock < 0)
            throw new ArgumentException("Minimum stock cannot be negative.");

        Name = name;
        SKU = sku;
        Description = description;
        Category = category;
        Price = price;
        MinimumStock = minimumStock;

        UpdatedAt = DateTime.UtcNow;
    }

    public void AddStock(int quantity) {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        StockQuantity += quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool RemoveStock(int quantity) {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        if (quantity > StockQuantity)
            return false;

        StockQuantity -= quantity;
        UpdatedAt = DateTime.UtcNow;

        return true;
    }
}