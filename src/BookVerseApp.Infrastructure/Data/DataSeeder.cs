using BookVerseApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookVerseApp.Infrastructure.Data;

public class DataSeeder
{
    private readonly AppDbContext _context;

    public DataSeeder(AppDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        if (await _context.Users.AnyAsync()) return;

        var users = new List<User>
        {
            new User
            {
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Role = "Admin"
            },
            new User
            {
                Username = "user",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("user123"),
                Role = "User"
            },
            new User
            {
                Username = "intern",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("intern123"),
                Role = "Intern"
            }
        };

        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();
    }
}
