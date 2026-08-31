using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using authapi.Models;
using Microsoft.IdentityModel.Tokens;

namespace authapi.Services;

public class JwtService {
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration) {
        _configuration = configuration;
    }

    public string GenerateToken(User user) {
        var key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key is missing");
        var issuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT issuer is missing");
        
        var claims = new[] {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id!),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Role),   
        };

        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key)
        );

        var credentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: issuer,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}