using Microsoft.EntityFrameworkCore;

namespace Portfoli.Data;

public class PortfoliDbContext(DbContextOptions<PortfoliDbContext> options) : DbContext(options)
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
