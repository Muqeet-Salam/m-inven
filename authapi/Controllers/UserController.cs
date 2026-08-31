using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace authapi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase {
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me() {
        return Ok(new {
            message = "You are authenticated"
        });
    }
}
