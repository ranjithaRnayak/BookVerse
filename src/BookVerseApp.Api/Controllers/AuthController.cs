using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookVerseApp.Application.DTOs;
using BookVerseApp.Domain.Entities;

namespace BookVerseApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;

    public AuthController(IConfiguration config)
    {
        _config = config;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] UserLoginDto loginDto)
    {
        var user = Authenticate(loginDto);

        if (user == null)
            return Unauthorized();

        var token = GenerateJwtToken(user);
        return Ok(new { token });
    }

    private User Authenticate(UserLoginDto login)
    {
        // 🔐 Simulate a user check (in real apps, use DB)
        if (login.Username == "admin" && login.Password == "admin123")
            return new User { Username = "admin", Role = "Admin" };

        if (login.Username == "user" && login.Password == "user123")
            return new User { Username = "user", Role = "User" };

        throw new UnauthorizedAccessException("Invalid credentials");
    }

    private string GenerateJwtToken(User user)
    {
        var jwtKey = _config["JwtSettings:Key"] ?? throw new InvalidOperationException("Missing JWT key");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

        var expiryStr = _config["JwtSettings:ExpiryMinutes"];
        var expiryMinutes = double.TryParse(expiryStr, out var value) ? value : 60;

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: _config["JwtSettings:Issuer"],
            audience: _config["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
