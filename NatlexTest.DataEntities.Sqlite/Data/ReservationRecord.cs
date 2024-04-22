using System.ComponentModel.DataAnnotations.Schema;

namespace NatlexTest.DataEntities.Sqlite;

[Table("BookHistory")]
public class ReservationRecord
{
    public int RecordId { get; set; }
    public string Changes { get; set; } = null!;
    [Column(TypeName = "datetime")]
    public DateTime Date { get; set; }

    public int BookId { get; set; }
    public Book Book { get; set; } = null!;
}
