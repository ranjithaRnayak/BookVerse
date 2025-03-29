using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using BookVerseApp.Application.DTOs;
using BookVerseApp.Domain.Entities;
using BookVerseApp.Infrastructure.Data;

namespace BookVerseApp.Api;
[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public BooksController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
    {
        var books = await _context.Books.ToListAsync();
        return Ok(books);
    }

    [HttpPost]
    public async Task<ActionResult<Book>> CreateBook([FromBody] Book book)
    {
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetBooks), new { id = book.Id }, book);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBook(Guid id, [FromBody] BookDto bookDto)
    {
        var existingBook = await _context.Books.FindAsync(id);
        if (existingBook == null)
            return NotFound();

        _mapper.Map(bookDto, existingBook); // maps updated fields into existing entity

        await _context.SaveChangesAsync();
        return NoContent(); // Standard REST response for update
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(Guid id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null)
            return NotFound();

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("bulk")]
    public async Task<IActionResult> CreateBooks([FromBody] List<BookDto> books)
    {
        var bookEntities = _mapper.Map<List<Book>>(books);
        _context.Books.AddRange(bookEntities);
        await _context.SaveChangesAsync();
        return Ok();
    }


}