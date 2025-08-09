using Microsoft.EntityFrameworkCore;

namespace Portfoli.Data;

public abstract class BaseDbContext<TDbContext>(DbContextOptions<TDbContext> options) : DbContext(options) where TDbContext : DbContext
{
    public DbSet<Portfolio> Portfolios { get; set; }

    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Portfolio>(entity =>
        {
            entity.ToTable("Portfolio");
            entity.HasKey(p => p.Id);

            entity
                .Property(p => p.Id)
                .HasConversion(id => id.Value, value => new PortfolioId(value));

            entity
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(256);

            entity
                .HasMany(p => p.Holdings)
                .WithOne()
                .HasForeignKey("PortfolioId")
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Holding>(entity =>
        {
            entity.ToTable("Holding");
            entity.HasKey(p => p.Id);

            entity
                .Property(p => p.Id)
                .HasConversion(id => id.Value, value => new HoldingId(value));

            entity.OwnsOne(
                p => p.Asset,
                asset =>
                {
                    asset
                        .Property(p => p.Exchange)
                        .IsRequired()
                        .HasMaxLength(128);

                    asset
                        .Property(p => p.Ticker)
                        .IsRequired()
                        .HasMaxLength(128);

                    asset
                        .Property(p => p.Name)
                        .IsRequired()
                        .HasMaxLength(512);

                    asset
                        .Property(p => p.Type)
                        .IsRequired()
                        .HasConversion<string>()
                        .HasMaxLength(32);
                });

            entity
                .Property(p => p.Quantity)
                .IsRequired();
        });

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

/// <summary>
/// DbContext for writing operations.
/// </summary>
/// <param name="options">The database context options.</param>
public class WritingDbContext(DbContextOptions<WritingDbContext> options) : BaseDbContext<WritingDbContext>(options)
{
}

/// <summary>
/// DbContext for reading operations.
/// </summary>
/// <param name="options">The database context options.</param>
public class ReadingDbContext(DbContextOptions<ReadingDbContext> options) : BaseDbContext<ReadingDbContext>(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

    public override int SaveChanges() => throw new NotSupportedException("This context is read-only.");

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => throw new NotSupportedException("This context is read-only.");
}
