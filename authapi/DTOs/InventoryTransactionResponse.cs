namespace authapi.DTOs;

public class InventoryTransactionResponse : InventoryTransactionDTO {    
    public string? Id { get; set; }
    public string Type { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

}