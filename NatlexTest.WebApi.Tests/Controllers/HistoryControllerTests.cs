using Moq;
using NatlexTest.WebApi.Controllers;
using NatlexTest.WebApi.Dto.History;
using NatlexTest.WebApi.Services.HistoryService;
using Microsoft.AspNetCore.Mvc;
using NatlexTest.DataEntities.Sqlite;

namespace NatlexTest.WebApi.Tests.Controllers;

public class HistoryControllerTests
{
    private readonly Mock<IHistoryService> _mockHistoryService;
    private readonly HistoryController _historyController;

    public HistoryControllerTests()
    {
        _mockHistoryService = new Mock<IHistoryService>();
        _historyController = new HistoryController(_mockHistoryService.Object);
    }

    [Fact]
    public async Task GetHistory_ShouldReturnOkWithListOfHistoryDto()
    {
        // Arrange
        var bookId = "123";
        var history = new List<BookHistory>
            {
                new() { HistoryId = 1, Date = DateTime.Now, BookId = bookId },
                new() { HistoryId = 2, Date = DateTime.Now, BookId = bookId }
            };

        _mockHistoryService.Setup(x => x.GetHistoryAsync(bookId))
            .ReturnsAsync(history);

        // Act
        var result = await _historyController.GetHistory(bookId);

        // Assert
        Assert.IsType<OkObjectResult>(result.Result);
        var historyDto = (result.Result as OkObjectResult)?.Value as IEnumerable<HistoryDto>;
        Assert.NotNull(historyDto);
        Assert.Equal(2, historyDto.Count());
    }

    [Fact]
    public async Task GetHistory_ShouldReturnNotFoundWhenHistoryIsNull()
    {
        // Arrange
        var bookId = "123";
        _mockHistoryService.Setup(x => x.GetHistoryAsync(bookId))
            .ReturnsAsync(Array.Empty<BookHistory>);

        // Act
        var result = await _historyController.GetHistory(bookId);

        // Assert
        Assert.IsType<OkObjectResult>(result.Result);
        var historyDto = (result.Result as OkObjectResult)?.Value as IEnumerable<HistoryDto>;
        Assert.NotNull(historyDto);
        Assert.Empty(historyDto);
    }
}
