using authapi.Models;
using authapi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace authapi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase {
    private readonly ProductService _productService;

    public ProductsController(ProductService productService) {
        _productService = productService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(Product product) {
        var createdProduct = await _productService.CreateAsync(product);

        return Ok(createdProduct);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() {
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }
}