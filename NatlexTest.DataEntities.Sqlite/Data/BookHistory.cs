using System.ComponentModel.DataAnnotations.Schema;

namespace NatlexTest.DataEntities.Sqlite;

[Table("BookHistories")]
public class BookHistory
{
    public int HistoryId { get; set; }
    public bool Reserved { get; set; }
    [Column(TypeName = "datetime")]
    public DateTime Date { get; set; }

    public string BookId { get; set; } = null!;
    public Book Book { get; set; } = null!;
}
