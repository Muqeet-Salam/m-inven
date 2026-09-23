using authapi.DTOs;
using authapi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace authapi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase {
    private readonly AuthService _authService;
    private readonly JwtService _jwtService;

    public AuthController(AuthService authService, JwtService jwtService) {
        _authService = authService;
        _jwtService = jwtService;
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
        var user = await _authService.LoginAsync(request);

        if (user == null) {
            return Unauthorized(new {
                message = "Invalid email or password"
            });
        }

        var token = _jwtService.GenerateToken(user);
        return Ok(new {
            message = "Login successful",
            token = token
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("users/{userId}/role")]
    public async Task<IActionResult> ChangeUserRole(
        string userId,
        ChangeRoleRequest request)
    {
        var result = await _authService.ChangeRoleAsync(
            userId,
            request.Role
        );

        if (!result)
        {
            return BadRequest(new
            {
                message = "Invalid role or user not found"
            });
        }

        return Ok(new
        {
            message = "User role updated successfully"
        });
    }
}


