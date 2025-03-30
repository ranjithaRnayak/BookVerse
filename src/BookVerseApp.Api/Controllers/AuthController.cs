using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookVerseApp.Application.DTOs;
using BookVerseApp.Domain.Entities;
using BookVerseApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using BookVerseApp.Application.Models;
using BookVerseApp.Application.Helpers;

namespace BookVerseApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthController> _logger;
    private readonly IWebHostEnvironment _env;

    public AuthController(AppDbContext context, IConfiguration config, ILogger<AuthController> logger, IWebHostEnvironment env)
    {
        _context = context;
        _config = config;
        _logger = logger;
        _env = env;
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<string>>> Login(UserLoginDto loginDto)
    {
        bool shouldLog = _env.IsDevelopment();

        var result = await ApiExecutor.Execute<string>(async () =>
          {
              var user = await Authenticate(loginDto);
              if (user == null)
                  return null!;

              return GenerateJwtToken(user);
          }, _logger, "Login", shouldLog, "Invalid username or password.");

        return Ok(result with { Message = "Login successful" });
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<string>>> Register(UserRegisterDto dto)
    {
        bool shouldLog = User.IsInRole("Admin") || _env.IsDevelopment();

        return Ok(await ApiExecutor.Execute(async () =>
        {
            var exists = await _context.Users.AnyAsync(u => u.Username == dto.Username);
            if (exists) return "Username already exists";

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return "User registered successfully";
        }, _logger, "Register", shouldLog));
    }
    private async Task<User?> Authenticate(UserLoginDto login)
    {
        bool shouldLog = _env.IsDevelopment();

        var result = await ApiExecutor.Execute(async () =>
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == login.Username);

            if (user is null) return null;

            return BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash) ? user : null;
        }, _logger, "Authenticate", shouldLog, "Invalid username or password.");
        return result.Data;
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
        _logger.LogInformation($"Generating token for user: {user.Username}, role: {user.Role}");
        return new JwtSecurityTokenHandler().WriteToken(token);

    }
}
