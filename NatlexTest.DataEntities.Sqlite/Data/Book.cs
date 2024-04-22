using System.ComponentModel.DataAnnotations.Schema;

namespace NatlexTest.DataEntities.Sqlite;

public class Book
{
    public int BookId { get; set; }
    [Column(TypeName = "varchar(128)")]
    public string Name { get; set; } = null!;
    public string Comment { get; set; } = null!;
    public bool IsReserved { get; set; }

    public ICollection<ReservationRecord> ReservationRecords { get; } = [];
}
