using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using BookVerseApp.Application.DTOs;
using BookVerseApp.Domain.Entities;
using BookVerseApp.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using BookVerseApp.Application.Models;
using BookVerseApp.Application.Helpers;
using System.Security.Claims;


namespace BookVerseApp.Api;
[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<BooksController> _logger;
    private readonly IWebHostEnvironment _env;

    public BooksController(AppDbContext context, IMapper mapper, ILogger<BooksController> logger, IWebHostEnvironment env)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
        _env = env;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<BookResponseDto>>>> GetBooks()
    {
        bool shouldLog = User.IsInRole("Admin") || _env.IsDevelopment();
        return Ok(await ApiExecutor.Execute(async () =>
         {
             var books = await _context.Books.ToListAsync();
             return _mapper.Map<List<BookResponseDto>>(books);
         }, _logger, "GetBooks", shouldLog));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<Book>> CreateBook([FromBody] BookDto bookDto)
    {
        _logger.LogInformation("User: " + User.Identity?.Name);
        var result = await ApiExecutor.Execute(async () =>
         {
             var book = _mapper.Map<Book>(bookDto);
             _context.Books.Add(book);
             await _context.SaveChangesAsync();
             return CreatedAtAction(nameof(GetBooks), new { id = book.Id }, book);
         }, _logger, "CreateBook", _env.IsDevelopment());
        return Ok(result);
    }

    [Authorize(Roles = "Admin, Intern")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBook(Guid id, [FromBody] BookDto bookDto)
    {
        var result = await ApiExecutor.Execute<bool>(async () =>
        {
            var existingBook = await _context.Books.FindAsync(id);
            if (existingBook == null)
                return false;
            bool shouldLog = User.IsInRole("Admin") || _env.IsDevelopment();


            _mapper.Map(bookDto, existingBook); // maps updated fields into existing entity

            await _context.SaveChangesAsync();
            return true; // Standard REST response for update
        }, _logger, "UpdateBook", _env.IsDevelopment(), "No Books found");
        if (!result.Success || result.Data == false)
            return NotFound(result);

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(Guid id)
    {
        var result = await ApiExecutor.Execute<bool>(async () =>
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return false;

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return true; // Standard REST response for update
        }, _logger, "UpdateBook", _env.IsDevelopment(), "No Books found");
        if (!result.Success || result.Data == false)
            return NotFound(result);

        return NoContent();
    }
    [Authorize(Roles = "Admin")]
    [HttpPost("bulk")]
    public async Task<IActionResult> CreateBooks([FromBody] List<BookDto> books)
    {
        return Ok(await ApiExecutor.Execute(async () =>
       {

           var bookEntities = _mapper.Map<List<Book>>(books);
           _context.Books.AddRange(bookEntities);
           await _context.SaveChangesAsync();
           return Ok();
       }, _logger, "CreateBooks", _env.IsDevelopment()));
    }

    // [Authorize]
    // [HttpGet("whoami")]
    // public IActionResult WhoAmI()
    // {
    //     return Ok(new
    //     {
    //         Username = User.Identity?.Name,
    //         Role = User.FindFirst(ClaimTypes.Role)?.Value
    //     });
    // }
}