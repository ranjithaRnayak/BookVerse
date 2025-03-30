using Microsoft.AspNetCore.Mvc;
using BookVerseApp.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using BookVerseApp.Application.Models;
using BookVerseApp.Application.Helpers;
using System.Security.Claims;
using BookVerseApp.Application.Interfaces;


namespace BookVerseApp.Api;
[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;
    private readonly ILogger<BooksController> _logger;
    private readonly IWebHostEnvironment _env;

    public BooksController(IBookService bookService, ILogger<BooksController> logger, IWebHostEnvironment env)
    {
        _bookService = bookService;
        _logger = logger;
        _env = env;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<BookResponseDto>>>> GetAllBooks()
    {
        return Ok(await ApiExecutor.Execute(() =>
            _bookService.GetAllAsync(),
            _logger, "GetAllBooks", _env.IsDevelopment()));
    }

    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<IEnumerable<BookResponseDto>>>> SearchBooks(
        [FromQuery] string? query,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        return Ok(await ApiExecutor.Execute(() =>
            _bookService.SearchAsync(query, page, pageSize),
            _logger, "SearchBooks", _env.IsDevelopment()));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<BookResponseDto>>> GetBookById(Guid id)
    {
        return Ok(await ApiExecutor.Execute(() =>
            _bookService.GetByIdAsync(id),
            _logger, "GetBookById", _env.IsDevelopment(), "Book not found"));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<BookResponseDto>>> CreateBook([FromBody] BookDto dto)
    {
        return Ok(await ApiExecutor.Execute(() =>
            _bookService.CreateAsync(dto),
            _logger, "CreateBook", _env.IsDevelopment()));
    }

    [Authorize(Roles = "Admin,Intern")]
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateBook(Guid id, [FromBody] BookDto dto)
    {
        var result = await ApiExecutor.Execute(() =>
            _bookService.UpdateAsync(id, dto),
            _logger, "UpdateBook", _env.IsDevelopment(), "Book not found");

        if (!result.Success || !result.Data)
            return NotFound(result);

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteBook(Guid id)
    {
        var result = await ApiExecutor.Execute(() =>
            _bookService.DeleteAsync(id),
            _logger, "DeleteBook", _env.IsDevelopment(), "Book not found");

        if (!result.Success || !result.Data)
            return NotFound(result);

        return NoContent();
    }

    [Authorize]
    [HttpGet("whoami")]
    public IActionResult WhoAmI()
    {
        return Ok(new
        {
            Username = User.Identity?.Name,
            Role = User.FindFirst(ClaimTypes.Role)?.Value
        });
    }
}
