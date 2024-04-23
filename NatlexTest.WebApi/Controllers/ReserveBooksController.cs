using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NatlexTest.DataContext;
using NatlexTest.DataEntities.Sqlite;
using NatlexTest.WebApi.Dto;
using NatlexTest.WebApi.Services.HistoryService;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NatlexTest.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReserveBooksController : ControllerBase
{
    private readonly BookContext _context;
    private readonly IHistoryService _historyService;

    public ReserveBooksController(BookContext context, IHistoryService historyService)
    {
        _context = context;
        _historyService = historyService;
    }

    /// <summary>
    /// Get books</br>
    /// By default return all reserved books
    /// GET: api/ReserveBooks
    /// </summary>
    /// <param name="reserved">By default true</param>
    /// <returns></returns>
    [HttpGet()]
    public async Task<ActionResult<IEnumerable<RetrieveReserveBookDto>>> Get([FromQuery] bool? reserved)
    {
        reserved ??= true;

        var books = await _context.Books
            .Where(b => b.Reserved == reserved)
            .ToListAsync();

        if (books.Count == 0)
        {
            return NoContent();
        }

        List<RetrieveReserveBookDto> reserveBooks = [];

        foreach (var book in books)
        {
            reserveBooks.Add(MapReserveBook(book));
        }

        return reserveBooks;
    }

    /// <summary>
    /// Reserve book</br>
    /// POST: api/ReserveBooks
    /// </summary>
    /// <param name="requestBook"></param>
    /// <returns></returns>
    [HttpPost()]
    public async Task<IActionResult> ReserveBook(RequestReserveBookDto requestBook)
    {
        var book = await _context.Books.FindAsync(requestBook.BookId);

        if (book == null)
        {
            return NotFound("Book with id " + requestBook.BookId + " not found");
        }

        if (book.Reserved)
        {
            return BadRequest("Book with id " + requestBook.BookId + " is already reserved");
        }

        book.Reserved = true;
        book.Comment = requestBook.Comment;
        _context.Entry(book).State = EntityState.Modified;

        await _historyService.PushHistoryAsync(book);
        await _context.SaveChangesAsync();

        return Ok();
    }

    /// <summary>
    /// Remove reserved status
    /// POST: api/ReserveBooks/remove
    /// </summary>
    /// <param name="id">Book id</param>
    /// <returns></returns>
    [HttpPost("delete/{id}")]
    public async Task<ActionResult<RetrieveReserveBookDto>> RemoveReserveStatus(string id)
    {
        var book = await _context.Books.FindAsync(id);

        if (book == null)
        {
            return NotFound("Book with id " + id + " not found");
        }

        book.Reserved = false;
        _context.Entry(book).State = EntityState.Modified;

        await _historyService.PushHistoryAsync(book);
        await _context.SaveChangesAsync();

        return MapReserveBook(book);
    }

    private RetrieveReserveBookDto MapReserveBook(Book book)
    {
        return new RetrieveReserveBookDto
        {
            BookId = book.BookId,
            Title = book.Title,
            Author = book.Author,
            Reserved = book.Reserved,
            Comment = book.Comment
        };
    }
}
