namespace authapi.DTOs;

public class ProductResponse : ProductDto {
    public string? Id { get; set; }
    public int StockQuantity { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}