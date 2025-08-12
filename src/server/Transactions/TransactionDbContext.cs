
namespace Portfoli.Transactions;

public class TransactionDbContext(DbContextOptions<TransactionDbContext> options) : DbContext(options)
{
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.ToTable("Transaction");
            entity.HasKey(p => p.Id);

            entity
                .Property(p => p.Id)
                .HasConversion(id => id.Value, value => new TransactionId(value));

            entity
                .Property(p => p.PortfolioId)
                .HasConversion(id => id.Value, value => new PortfolioId(value))
                .IsRequired();

            entity
                .HasOne<Portfolio>()
                .WithMany()
                .HasForeignKey(p => p.PortfolioId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .Property(p => p.HoldingId)
                .HasConversion(id => id.Value, value => new HoldingId(value))
                .IsRequired();

            entity
                .HasOne<Holding>()
                .WithMany()
                .HasForeignKey(p => p.HoldingId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .Property(p => p.Type)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(8);

            entity
                .Property(p => p.Date)
                .IsRequired();

            entity
                .Property(p => p.Quantity)
                .IsRequired();

            entity
                .Property(p => p.Price)
                .IsRequired();

            entity
                .Property(p => p.Commission)
                .IsRequired();
        });
    }
}
