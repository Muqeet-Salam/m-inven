namespace authapi.DTOs;

public class InventoryTransactionDTO {
    public string ProductId { get; set; } = null!;
    public int Quantity { get; set; }
    public string? Reason { get; set; }
}