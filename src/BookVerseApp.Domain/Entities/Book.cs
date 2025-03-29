namespace BookVerseApp.Domain.Entities;

public class Book
{
    public Guid Id { get; set; } = Guid.NewGuid(); // Primary Key
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public double Rating { get; set; }
    public bool IsAvailable { get; set; } = true;
}
