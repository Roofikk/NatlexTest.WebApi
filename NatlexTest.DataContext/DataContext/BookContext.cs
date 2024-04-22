using Microsoft.EntityFrameworkCore;
using NatlexTest.DataEntities.Sqlite;

namespace NatlexTest.DataContext;

public class BookContext : DbContext
{
    public DbSet<Book> Books { get; set; }
    public DbSet<ReservationRecord> History { get; set; }

    public BookContext()
        : base()
    {
    }

    public BookContext(DbContextOptions<BookContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=../books.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Book>(e =>
        {
            e.HasKey(e => e.BookId);
            e.Property(e => e.BookId).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<ReservationRecord>(e =>
        {
            e.HasKey(e => e.RecordId);
            e.Property(e => e.BookId).ValueGeneratedOnAdd();

            e.HasOne(e => e.Book)
                .WithMany(e => e.ReservationRecords)
                .HasForeignKey(e => e.BookId)
                .IsRequired();
        });
    }
}
