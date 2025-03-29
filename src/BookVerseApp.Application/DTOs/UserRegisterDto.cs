namespace BookVerseApp.Application.DTOs;

public class UserRegisterDto : UserAuthBaseDto
{
    public string Role { get; set; } = "User"; // Default to regular user
}
