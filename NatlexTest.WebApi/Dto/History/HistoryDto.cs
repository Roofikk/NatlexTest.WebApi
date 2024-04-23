namespace NatlexTest.WebApi.Dto.History;

public class HistoryDto
{
    public int HistoryId { get; set; }
    public string BookId { get; set; } = null!;
    public DateTime Date { get; set; }
    public bool Reserved { get; set; }
}
