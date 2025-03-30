
using BookVerseApp.Application.DTOs;
namespace BookVerseApp.Application.Interfaces;
public interface IBookService
{
    Task<IEnumerable<BookResponseDto>> GetAllAsync();
    Task<IEnumerable<BookResponseDto>> SearchAsync(string? query, int page, int pageSize);
    Task<BookResponseDto?> GetByIdAsync(Guid id);
    Task<BookResponseDto> CreateAsync(BookDto dto);
    Task<bool> UpdateAsync(Guid id, BookDto dto);
    Task<bool> DeleteAsync(Guid id);
}
