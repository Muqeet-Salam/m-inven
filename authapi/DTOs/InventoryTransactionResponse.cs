namespace authapi.DTOs;

public class InventoryTransactionResponse {    
    public string? Id { get; set; }
    public string ProductId { get; set; } = null!;
    public string Type { get; set; } = null!;
    public int Quantity { get; set; }
    public string? Reason { get; set; }
    public string UserId { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

}