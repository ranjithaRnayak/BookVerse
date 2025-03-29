using System;

namespace BookVerseApp.Domain.Entities;

public class User
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Role { get; set; } = "User"; // or "Admin"
}
