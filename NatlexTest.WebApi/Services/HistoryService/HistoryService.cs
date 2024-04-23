using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NatlexTest.DataContext;
using NatlexTest.DataEntities.Sqlite;

namespace NatlexTest.WebApi.Services.HistoryService;

public class HistoryService : IHistoryService
{
    private readonly BookContext _context;

    public HistoryService(BookContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BookHistory>> GetHistoryAsync(string bookId)
    {
        return await _context.Histories
            .Where(x => x.BookId == bookId)
            .ToListAsync();
    }

    /// <summary>
    /// Add all changed properties
    /// </summary>
    /// <param name="book"></param>
    /// <returns></returns>
    public async Task PushHistoryAsync(Book book)
    {
        var lastHistory = await _context.Histories
            .OrderByDescending(x => x.Date)
            .FirstOrDefaultAsync();

        if (lastHistory == null)
        {
            _context.Histories.Add(new BookHistory
            {
                BookId = book.BookId,
                Reserved = book.Reserved,
                Date = DateTime.Now,
            });

            return;
        }

        if (lastHistory.Reserved != book.Reserved)
        {
            _context.Histories.Add(new BookHistory
            {
                BookId = book.BookId,
                Reserved = book.Reserved,
                Date = DateTime.Now,
            });
        }
    }

    public async Task<bool> SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
        return true;
    }
}
