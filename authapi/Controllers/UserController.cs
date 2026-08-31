using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace authapi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase {
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me() {
        return Ok(new {
            message = "You are authenticated",
            name = User.Identity?.Name,
            role = User.FindFirst(ClaimTypes.Role)?.Value
        });
    }
    [Authorize(Roles = "Admin")]
    [HttpGet("admin")]
    public IActionResult AdminOnly()
    {
        return Ok(new
        {
            message = "You are an Admin!"
        });
    }
}
