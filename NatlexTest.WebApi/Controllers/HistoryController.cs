using Microsoft.AspNetCore.Mvc;
using NatlexTest.WebApi.Dto.History;
using NatlexTest.WebApi.Services.HistoryService;

namespace NatlexTest.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HistoryController : ControllerBase
{
    private readonly IHistoryService _historyService;

    public HistoryController(IHistoryService historyService)
    {
        _historyService = historyService;
    }

    /// <summary>
    /// Get history of book reservations
    /// </summary>
    /// <param name="bookId">Book id</param>
    /// <returns></returns>
    [HttpGet("history/{bookId}")]
    public async Task<ActionResult<IEnumerable<HistoryDto>>> GetHistory(string bookId)
    {
        var histories = (await _historyService
            .GetHistoryAsync(bookId))
            .Select(x => new HistoryDto
            {
                HistoryId = x.HistoryId,
                BookId = x.BookId,
                Date = x.Date,
                Reserved = x.Reserved
            });

        return Ok(histories);
    }
}
