using System.ComponentModel.DataAnnotations.Schema;

namespace NatlexTest.DataEntities.Sqlite;

public class Book
{
    [Column(TypeName = "varchar(40)")]
    public string BookId { get; set; } = null!;
    [Column(TypeName = "varchar(128)")]
    public string Title { get; set; } = null!;
    public string? Author { get; set; }
    [Column(TypeName = "text")]
    public string Comment { get; set; } = "";
    [Column(TypeName = "integer")]
    public bool Reserved { get; set; } = false;

    public ICollection<BookHistory> Histories { get; } = [];
}
