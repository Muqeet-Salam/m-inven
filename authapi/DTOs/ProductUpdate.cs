namespace authapi.DTOs;

public class ProductUpdate {
    public string Name { get; set; } = null!;
    public string SKU { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Category { get; set; } = null!;
    public decimal Price { get; set; }
    public int MinimumStock { get; set; }
}