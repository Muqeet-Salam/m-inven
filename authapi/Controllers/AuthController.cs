using authapi.DTOs;
using authapi.Services;
using Microsoft.AspNetCore.Mvc;

namespace authapi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase {
    private readonly AuthService _authService;

    public AuthController(AuthService authService) {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request) {
        var result = await _authService.RegisterAsync(request);

        if (!result) {
            return BadRequest(new {
                message = "Email is already registered"
            });
        }

        return Ok(new {
            message = "Registration successful"
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request) {
        var result = await _authService.LoginAsync(request);

        if (!result) {
            return Unauthorized(new {
                message = "Invalid email or password"
            });
        }

        return Ok(new {
            message = "Login successful"
        });
    }
}


