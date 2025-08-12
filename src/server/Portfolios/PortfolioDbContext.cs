namespace Portfoli.Portfolios;

public class PortfolioDbContext(DbContextOptions<PortfolioDbContext> options) : DbContext(options)
{
    public DbSet<Portfolio> Portfolios { get; set; }

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
    }
}
