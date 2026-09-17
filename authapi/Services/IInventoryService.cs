using authapi.DTOs;

namespace authapi.Services;

public interface IInventoryService {
    Task<bool> StockInAsync(InventoryTransactionDTO dto, string userId);

    Task<bool> StockOutAsync(InventoryTransactionDTO dto, string userId);
}