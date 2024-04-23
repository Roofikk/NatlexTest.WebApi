using Microsoft.EntityFrameworkCore;
using NatlexTest.DataEntities.Sqlite;

namespace NatlexTest.DataContext;

public class BookContext : DbContext
{
    public DbSet<Book> Books { get; set; }
    public DbSet<BookHistory> Histories { get; set; }

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
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=../books.db");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Book>(e =>
        {
            e.HasKey(e => e.BookId);
        });

        modelBuilder.Entity<BookHistory>(e =>
        {
            e.HasKey(e => e.HistoryId);
            e.Property(e => e.BookId).ValueGeneratedOnAdd();

            e.HasOne(e => e.Book)
                .WithMany(e => e.Histories)
                .HasForeignKey(e => e.BookId)
                .IsRequired();
        });
    }
}
