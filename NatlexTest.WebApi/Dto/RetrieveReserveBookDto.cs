using NatlexTest.DataEntities.Sqlite;

namespace NatlexTest.WebApi.Dto;

public class RetrieveReserveBookDto
{
    public string BookId { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string? Author { get; set; }
    public bool Reserved { get; set; }
    public string Comment { get; set; } = null!;
}
