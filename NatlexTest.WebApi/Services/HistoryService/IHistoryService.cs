using NatlexTest.DataEntities.Sqlite;

namespace NatlexTest.WebApi.Services.HistoryService;

public interface IHistoryService
{
    public Task<IEnumerable<BookHistory>> GetHistoryAsync(string bookId);
    public Task PushHistoryAsync(Book book);
    public Task<bool> SaveChangesAsync();
}
