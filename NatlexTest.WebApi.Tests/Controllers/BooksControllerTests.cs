using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NatlexTest.DataContext;
using NatlexTest.DataEntities.Sqlite;
using NatlexTest.WebApi.Controllers;
using NatlexTest.WebApi.Dto;

namespace NatlexTest.WebApi.Tests.Controllers;

public class BooksControllerTests
{
    private readonly BooksController _booksController;
    private readonly BookContext _context;

    public BooksControllerTests()
    {
        var dbContextOptions = new DbContextOptionsBuilder<BookContext>()
            .UseInMemoryDatabase(databaseName: $"InMemoryBookDatabase_{Guid.NewGuid()}")
            .Options;

        _context = new BookContext(dbContextOptions);
        _booksController = new BooksController(_context);
    }

    [Fact]
    public async Task GetBooks_ShouldReturnOkWithListOfBookDto()
    {
        // Arrange
        var book1 = new Book { BookId = "1", Title = "Book 1" };
        var book2 = new Book { BookId = "2", Title = "Book 2" };

        await _context.Books.AddRangeAsync(book1, book2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _booksController.GetBooks();

        // Assert
        Assert.IsType<OkObjectResult>(result.Result);
        var booksDto = (result.Result as OkObjectResult)?.Value as List<RetrieveBookDto>;
        Assert.NotNull(booksDto);
        Assert.Equal(2, booksDto.Count);
        Assert.Equal(book1.BookId, booksDto[0].BookId);
        Assert.Equal(book1.Title, booksDto[0].Title);
        Assert.Equal(book2.BookId, booksDto[1].BookId);
        Assert.Equal(book2.Title, booksDto[1].Title);
    }

    [Fact]
    public async Task GetBooks_ShouldReturnOkWithEmptyList()
    {
        // Arrange

        // Act
        var result = await _booksController.GetBooks();

        // Assert
        Assert.IsType<NoContentResult>(result.Result);
        Assert.Empty(await _context.Books.ToListAsync());
    }

    [Fact]
    public async Task GetBook_ShouldReturnOkWithBookDto()
    {
        // Arrange
        var book = new Book { BookId = "1", Title = "Book 1" };
        await _context.Books.AddAsync(book);
        await _context.SaveChangesAsync();

        // Act
        var result = await _booksController.GetBook(book.BookId);

        // Assert
        Assert.IsType<OkObjectResult>(result.Result);
        var bookDto = (result.Result as OkObjectResult)?.Value as RetrieveBookDto;
        Assert.NotNull(bookDto);
        Assert.Equal(book.BookId, bookDto.BookId);
        Assert.Equal(book.Title, bookDto.Title);
    }

    [Fact]
    public async Task GetBook_ShouldReturnNotFound()
    {
        // Arrange
        var book = new Book { BookId = "1", Title = "Book 1" };
        await _context.Books.AddAsync(book);
        await _context.SaveChangesAsync();

        // Act
        var result = await _booksController.GetBook("2");

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task AddBook_ShouldReturnOkWithBookDto()
    {
        // Arrange
        var book = new RequestBookDto { BookId = "1", Title = "Book 1", Author = "Author 1" };

        // Act
        var result = await _booksController.CreateBook(book);

        // Assert
        Assert.IsType<CreatedAtActionResult>(result.Result);
        var createdResult = result.Result as CreatedAtActionResult;
        Assert.NotNull(createdResult);
        Assert.Equal("GetBook", createdResult.ActionName);
        Assert.NotNull(createdResult.RouteValues);
        Assert.True(createdResult.RouteValues.ContainsKey("bookId"));
        Assert.Equal("1", createdResult.RouteValues["bookId"]);
        var bookDto = (createdResult.Value as RetrieveBookDto);
        Assert.NotNull(bookDto);
        Assert.Equal(book.BookId, bookDto.BookId);
        Assert.Equal(book.Title, bookDto.Title);
        Assert.Equal(book.Author, bookDto.Author);
    }

    [Fact]
    public async Task AddBook_ShouldReturnBadRequest()
    {
        // Arrange
        var book = new Book { BookId = "1", Title = "Book 1" };
        var requestBook = new RequestBookDto { BookId = "1", Title = "Book 2" };
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        _context.Entry(book).State = EntityState.Detached;

        // Act
        var result = await _booksController.CreateBook(requestBook);

        // Assert
        Assert.IsType<ConflictObjectResult>(result.Result);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task UpdateBook_ShouldReturnOkWithBookDto()
    {
        // Arrange
        var book = new Book { BookId = "1", Title = "Book 1" };
        await _context.Books.AddAsync(book);
        await _context.SaveChangesAsync();

        // Act
        var result = await _booksController.UpdateBook(new RequestBookDto { BookId = "1", Title = "First book" });

        // Assert
        Assert.IsType<OkObjectResult>(result.Result);
        var bookDto = (result.Result as OkObjectResult)?.Value as RetrieveBookDto;
        Assert.NotNull(bookDto);
        Assert.Equal(book.BookId, bookDto.BookId);
        Assert.Equal("First book", bookDto.Title);
    }

    [Fact]
    public async Task UpdateBook_ShouldReturnNotFound()
    {
        // Arrange
        var book = new Book { BookId = "1", Title = "Book 1" };
        await _context.Books.AddAsync(book);
        await _context.SaveChangesAsync();

        // Act
        var result = await _booksController.UpdateBook(new RequestBookDto { BookId = "2", Title = "Second book" });

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task DeleteBook_ShouldReturnOk()
    {
        // Arrange
        var book = new Book { BookId = "1", Title = "Book 1" };
        await _context.Books.AddAsync(book);
        await _context.SaveChangesAsync();

        // Act
        var result = await _booksController.DeleteBook(book.BookId);

        // Assert
        Assert.IsType<NoContentResult>(result);
        Assert.Empty(await _context.Books.ToListAsync());
    }

    [Fact]
    public async Task DeleteBook_ShouldReturnNotFound()
    {
        // Arrange
        var book = new Book { BookId = "1", Title = "Book 1" };
        await _context.Books.AddAsync(book);
        await _context.SaveChangesAsync();

        // Act
        var result = await _booksController.DeleteBook("2");

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}