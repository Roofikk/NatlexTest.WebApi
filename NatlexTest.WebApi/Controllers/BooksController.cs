using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NatlexTest.DataContext;
using NatlexTest.DataEntities.Sqlite;
using NatlexTest.WebApi.Dto;
using NatlexTest.WebApi.Services.HistoryService;

namespace NatlexTest.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BooksController : ControllerBase
{
    private readonly BookContext _context;

    public BooksController(BookContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all books</br>
    /// GET: api/Books
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RetrieveBookDto>>> GetBooks()
    {
        var books = await _context.Books.ToListAsync();

        if (books.Count == 0)
        {
            return NoContent();
        }

        var booksDto = new List<RetrieveBookDto>();

        foreach (var book in books)
        {
            booksDto.Add(MapBook(book));
        }

        return Ok(booksDto);
    }

    /// <summary>
    /// Get book</br>
    /// GET: api/Books/5
    /// </summary>
    /// <param name="bookId">Book id</param>
    /// <returns></returns>
    [HttpGet("{bookId}")]
    public async Task<ActionResult<RetrieveBookDto>> GetBook(string bookId)
    {
        var book = await _context.Books.FindAsync(bookId);

        if (book == null)
        {
            return NotFound();
        }

        return Ok(MapBook(book));
    }

    /// <summary>
    /// Create book</br>
    /// POST: api/Books
    /// </summary>
    /// <param name="book"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<RetrieveBookDto>> CreateBook(RequestBookDto requestBook)
    {
        if (string.IsNullOrEmpty(requestBook.BookId))
        {
            requestBook.BookId = Guid.NewGuid().ToString();
        }

        if (BookExists(requestBook.BookId))
        {
            return Conflict("Book with id " + requestBook.BookId + " is already exists");
        }

        var createdBook = new Book
        {
            BookId = requestBook.BookId,
            Title = requestBook.Title,
            Author = requestBook.Author
        };

        _context.Books.Add(createdBook);

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBook), new { bookId = createdBook.BookId }, MapBook(createdBook));
    }

    /// <summary>
    /// Update book</br>
    /// PUT: api/Books
    /// </summary>
    /// <param name="bookDto"></param>
    /// <returns></returns>
    [HttpPut()]
    public async Task<ActionResult<RetrieveBookDto>> UpdateBook(RequestBookDto requestBook)
    {
        if (string.IsNullOrEmpty(requestBook.BookId))
        {
            return BadRequest("Book id is required");
        }

        var book = await _context.Books.FindAsync(requestBook.BookId);

        if (book == null)
        {
            return NotFound("Book with id " + requestBook.BookId + " not found");
        }

        book.Title = requestBook.Title;
        book.Author = requestBook.Author;

        await _context.SaveChangesAsync();
        return Ok(MapBook(book));
    }

    // DELETE: api/Books/5
    [HttpDelete("{bookId}")]
    public async Task<IActionResult> DeleteBook(string bookId)
    {
        var book = await _context.Books.FindAsync(bookId);

        if (book == null)
        {
            return NotFound();
        }

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool BookExists(string id)
    {
        return _context.Books.Any(e => e.BookId == id);
    }

    private RetrieveBookDto MapBook(Book book)
    {
        return new RetrieveBookDto
        {
            BookId = book.BookId,
            Title = book.Title,
            Author = book.Author
        };
    }
}
