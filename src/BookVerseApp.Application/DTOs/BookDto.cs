using System.ComponentModel.DataAnnotations;
namespace BookVerseApp.Application.DTOs;

public class BookDto
{
    public Guid Id { get; set; }
    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;
    [Required]
    [StringLength(100)]
    public string Author { get; set; } = string.Empty;
    [Required]
    [StringLength(50)]
    public string Genre { get; set; } = string.Empty;
    [Required]
    [RegularExpression(@"^\d{10,13}$", ErrorMessage = "ISBN must be 10 to 13 digits.")]
    public string ISBN { get; set; } = string.Empty;
    [Range(0, 5)]
    public double Rating { get; set; }
    public bool IsAvailable { get; set; }
}
