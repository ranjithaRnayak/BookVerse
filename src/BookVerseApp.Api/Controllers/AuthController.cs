using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookVerseApp.Application.DTOs;
using BookVerseApp.Domain.Entities;
using BookVerseApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookVerseApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly AppDbContext _context;
   // private readonly IMapper _mapper;


    public AuthController(IConfiguration config, AppDbContext appDbContext)
    {
        _config = config;
        _context = appDbContext;
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto registerDto)
    {
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == registerDto.Username);

        if (existingUser != null)
            return BadRequest("User already exists");

        var user = new User
        {
            Username = registerDto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
            Role = registerDto.Role
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok("User registered successfully");
    }

    [HttpPost("login")]
   public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
    {
        var user = await Authenticate(loginDto);

        if (user == null)
            return Unauthorized();

        var token = GenerateJwtToken(user);
        return Ok(new { token });
    }

   private async Task<User?> Authenticate(UserLoginDto login)
{
    var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == login.Username);
    
    if (user is null) return null;

    return BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash) ? user : null;
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
