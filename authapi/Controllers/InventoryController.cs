using authapi.DTOs;
using authapi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace authapi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]

public class InventoryController : ControllerBase {
    
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService) {
        _inventoryService = inventoryService;
    }

    [Authorize(Roles = "Admin,Manager,Staff")]
    [HttpPost("{productId}/stock-in")]
    public async Task<IActionResult> StockIn(
        string productId,
        int quantity,
        string? reason) {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
            return Unauthorized();
        
        var dto = new InventoryTransactionDTO {
            ProductId = productId,
            Quantity = quantity,
            Reason = reason,
        };

        var result = await _inventoryService.StockInAsync(dto, userId);

        if (!result) {
            return BadRequest(new{ message = "Product not found" });
        }

        return Ok(new{ message = "Stock added successfully" });
    }

    [Authorize(Roles = "Admin,Manager,Staff")]
    [HttpPost("{productId}/stock-out")]
    public async Task<IActionResult> StockOut(
        string productId,
        int quantity,
        string? reason) {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
            return Unauthorized();

        var dto = new InventoryTransactionDTO {
            ProductId = productId,
            Quantity = quantity,
            Reason = reason
        };

        var result = await _inventoryService.StockOutAsync(dto, userId);

        if (!result) {
            return BadRequest(new { message = "Product not found or insufficient stock" } );
        }

        return Ok(new { message = "Stock removed successfully" } );
    }
}