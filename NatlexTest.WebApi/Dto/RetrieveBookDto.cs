namespace NatlexTest.WebApi.Dto;

public class RetrieveBookDto
{
    public string BookId { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string? Author { get; set; }
}
