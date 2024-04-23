using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NatlexTest.DataContext;
using NatlexTest.DataEntities.Sqlite;
using NatlexTest.WebApi.Controllers;
using NatlexTest.WebApi.Dto;
using NatlexTest.WebApi.Services.HistoryService;

namespace NatlexTest.WebApi.Tests.Controllers;

public class ReserveBooksControllerTests
{
    private readonly BookContext _context;
    private readonly Mock<IHistoryService> _mockHistoryService;
    private readonly ReserveBooksController _reserveController;

    public ReserveBooksControllerTests()
    {
        var dbContextOptions = new DbContextOptionsBuilder<BookContext>()
            .UseInMemoryDatabase(databaseName: $"InMemoryBookDatabase_{Guid.NewGuid()}")
            .Options;

        _context = new BookContext(dbContextOptions);

        _mockHistoryService = new Mock<IHistoryService>();
        _reserveController = new ReserveBooksController(_context, _mockHistoryService.Object);
    }

    [Fact]
    public async Task GetReservedBooks_ShouldReturnOk()
    {
        // Arrange
        var book1 = new Book { BookId = "1", Title = "Book 1", Reserved = true, Comment = "Comment 1" };
        var book2 = new Book { BookId = "2", Title = "Book 2", Reserved = false, Comment = "Comment 1" };
        await _context.Books.AddRangeAsync(book1, book2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _reserveController.GetBooks();

        // Assert
        Assert.IsType<OkObjectResult>(result.Result);
        var booksDto = (result.Result as OkObjectResult)?.Value as List<RetrieveReserveBookDto>;
        Assert.NotNull(booksDto);
        Assert.Single(booksDto);
        Assert.Equal(book1.BookId, booksDto[0].BookId);
        Assert.Equal(book1.Title, booksDto[0].Title);
        Assert.Equal(book1.Comment, booksDto[0].Comment);
        Assert.Equal(book1.Reserved, booksDto[0].Reserved);
    }

    [Fact]
    public async Task GetAvailableBooks_ShouldReturnOk()
    {
        // Arrange
        var book1 = new Book { BookId = "1", Title = "Book 1", Reserved = true, Comment = "Comment 1" };
        var book2 = new Book { BookId = "2", Title = "Book 2", Reserved = false, Comment = "Comment 1" };
        await _context.Books.AddRangeAsync(book1, book2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _reserveController.GetBooks(false);

        // Assert
        Assert.IsType<OkObjectResult>(result.Result);
        var booksDto = (result.Result as OkObjectResult)?.Value as List<RetrieveReserveBookDto>;
        Assert.NotNull(booksDto);
        Assert.Single(booksDto);
        Assert.Equal(book2.BookId, booksDto[0].BookId);
        Assert.Equal(book2.Title, booksDto[0].Title);
        Assert.Equal(book2.Comment, booksDto[0].Comment);
        Assert.Equal(book2.Reserved, booksDto[0].Reserved);
    }

    [Fact]
    public async Task ReserveBook_ShouldReturnOk()
    {
        // Arrange
        var book = new Book { BookId = "1", Title = "Book 1" };
        await _context.Books.AddAsync(book);
        await _context.SaveChangesAsync();

        // Act
        var result = await _reserveController.ReserveBook(new RequestReserveBookDto { BookId = book.BookId, Comment = "Comment 1" });

        // Assert
        Assert.IsType<OkObjectResult>(result.Result);
        var reservedBook = (result.Result as OkObjectResult)?.Value as RetrieveReserveBookDto;
        Assert.NotNull(reservedBook);
        Assert.True(reservedBook.Reserved);
        Assert.Equal(book.BookId, reservedBook.BookId);
        Assert.Equal(book.Title, reservedBook.Title);
        Assert.Equal("Comment 1", reservedBook.Comment);
    }

    [Fact]
    public async Task ReserveBook_ShouldReturnBadRequest()
    {
        // Arrange
        var book = new Book { BookId = "1", Title = "Book 1", Reserved = true, Comment = "Comment 1" };
        await _context.Books.AddAsync(book);
        await _context.SaveChangesAsync();

        // Act
        var result = await _reserveController.ReserveBook(new RequestReserveBookDto { BookId = book.BookId, Comment = "Comment 1" });

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task ReserveBook_ShouldReturnNotFound()
    {
        // Arrange
        var book = new Book { BookId = "1", Title = "Book 1", Reserved = true, Comment = "Comment 1" };
        await _context.Books.AddAsync(book);
        await _context.SaveChangesAsync();

        // Act
        var result = await _reserveController.ReserveBook(new RequestReserveBookDto { BookId = "2", Comment = "Comment 1" });

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task UnreserveBook_ShouldReturnOk()
    {
        // Arrange
        var book = new Book { BookId = "1", Title = "Book 1", Reserved = true, Comment = "Comment 1" };
        await _context.Books.AddAsync(book);
        await _context.SaveChangesAsync();

        // Act
        var result = await _reserveController.RemoveReserveStatus(book.BookId);

        // Assert
        Assert.IsType<OkObjectResult>(result.Result);
        var reservedBook = (result.Result as OkObjectResult)?.Value as RetrieveReserveBookDto;
        Assert.NotNull(reservedBook);
        Assert.False(reservedBook.Reserved);
        Assert.Equal(book.BookId, reservedBook.BookId);
        Assert.Equal(book.Title, reservedBook.Title);
        Assert.Equal("Comment 1", reservedBook.Comment);
    }
}
