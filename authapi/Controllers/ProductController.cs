using authapi.DTOs;
using authapi.Models;
using authapi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace authapi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase {
    private readonly IProductService _productService;

    public ProductsController(IProductService productService) {
        _productService = productService;
    }

    [Authorize(Roles = "Admin, Manager")]
    [HttpPost]
    public async Task<IActionResult> Create(ProductCreate dto) {
        var createdProduct = await _productService.CreateAsync(dto);

        return Ok(createdProduct);
    }

    [HttpGet("{name}")]
    public async Task<IActionResult> GetByName(string name) {
        var product = await _productService.GetByNameAsync(name);
        if (product == null) {
            return NotFound(new
            {
                message = "Product not found"
            });
        }
        return Ok(product);
    }

    [Authorize(Roles = "Admin, Manager")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, ProductUpdate dto) {
        var updated = await _productService.UpdateAsync(id, dto);

        if (!updated) {
            return NotFound(new {
                message = "product not found"
            });
        }

        return Ok(new{ message = "Product updated successfully" });
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete]
    public async Task<IActionResult> Delete(string name) {
        var deleted = await _productService.DeleteAsync(name);

        if (!deleted) return NotFound(new { message = "Product not found" });
        return Ok(new { message = "Product Deleted" });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() {
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }
}