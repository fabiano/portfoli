using Microsoft.EntityFrameworkCore;

namespace Portfoli.Data;

public abstract class BaseDbContext<TDbContext>(DbContextOptions<TDbContext> options) : DbContext(options) where TDbContext : DbContext
{
    public DbSet<Portfolio> Portfolios { get; set; }

    public DbSet<Asset> Assets { get; set; }

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
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Holding>(entity =>
        {
            entity.ToTable("Holding");
            entity.HasKey(p => p.Id);

            entity
                .Property(p => p.Id)
                .HasConversion(id => id.Value, value => new HoldingId(value));

            entity
                .HasOne(p => p.Asset)
                .WithMany()
                .HasForeignKey("AssetId");

            entity
                .Property(p => p.Quantity)
                .IsRequired();

            entity
                .HasMany(p => p.Transactions)
                .WithOne()
                .HasForeignKey("HoldingId")
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.ToTable("Transaction");
            entity.HasKey(p => p.Id);

            entity
                .Property(p => p.Id)
                .HasConversion(id => id.Value, value => new TransactionId(value));

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

        modelBuilder.Entity<Asset>(entity =>
        {
            entity.ToTable("Asset");
            entity.HasKey(p => p.Id);

            entity
                .Property(p => p.Id)
                .HasConversion(id => id.Value, value => new AssetId(value));

            entity
                .Property(p => p.Exchange)
                .IsRequired()
                .HasMaxLength(128);

            entity
                .Property(p => p.Ticker)
                .IsRequired()
                .HasMaxLength(128);

            entity
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(512);

            entity
                .Property(p => p.AssetType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(32);
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
