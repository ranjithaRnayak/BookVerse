using AutoMapper;
using BookVerseApp.Application.DTOs;
using BookVerseApp.Application.Interfaces;
using BookVerseApp.Domain.Entities;
using BookVerseApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookVerseApp.Application.Services;

public class BookService : IBookService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public BookService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BookResponseDto>> GetAllAsync()
    {
        var books = await _context.Books.ToListAsync();
        return _mapper.Map<List<BookResponseDto>>(books);
    }

    public async Task<IEnumerable<BookResponseDto>> SearchAsync(string? query, int page, int pageSize)
    {
        var books = _context.Books.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            books = books.Where(b =>
                b.Title.Contains(query) ||
                b.Author.Contains(query) ||
                b.Genre.Contains(query));
        }

        var paginated = await books
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return _mapper.Map<List<BookResponseDto>>(paginated);
    }

    public async Task<BookResponseDto?> GetByIdAsync(Guid id)
    {
        var book = await _context.Books.FindAsync(id);
        return book == null ? null : _mapper.Map<BookResponseDto>(book);
    }

    public async Task<BookResponseDto> CreateAsync(BookDto dto)
    {
        var book = _mapper.Map<Book>(dto);
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return _mapper.Map<BookResponseDto>(book);
    }

    public async Task<bool> UpdateAsync(Guid id, BookDto dto)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null) return false;

        _mapper.Map(dto, book);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null) return false;

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return true;
    }
}
