using authapi.Data;
using Microsoft.AspNetCore.Mvc;

namespace authapi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly MongoDbContext _mongoDb;

    public TestController(MongoDbContext mongoDb) {
        _mongoDb = mongoDb;
    }

    [HttpGet]
    public IActionResult Test() {
        return Ok(new {
            message = "API is working!"
        });
    }
    [HttpGet("database")]
    public async Task<IActionResult> TestDatabase() {
        var connected = await _mongoDb.PingAsync();
        if (!connected) {
            return StatusCode(500, new {
                message = "Could not connect to MongoDB Atlas"
            });
        }

        return Ok(new {
            message = "Successfully connected to MongoDB Atlas!"
        });
    }
}