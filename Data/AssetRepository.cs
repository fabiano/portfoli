using Microsoft.EntityFrameworkCore;

namespace Portfoli.Data;

/// <summary>
/// Implementation of the <see cref="IAssetRepository"> interface using the Entity Framework database context.
/// </summary>
/// <param name="dbContext">The database context.</param>
public class AssetRepository(PortfoliDbContext dbContext) : IAssetRepository
{
    /// <inheritdoc />
    public async Task<Asset?> Get(AssetId id) => await dbContext.Assets.FindAsync(id);

    /// <inheritdoc />
    public async Task<Asset?> GetByTicker(string exchange, string ticker) => await dbContext.Assets.FirstOrDefaultAsync(a => a.Exchange == exchange && a.Ticker == ticker);

    /// <inheritdoc />
    public async Task Add(Asset asset) => await dbContext.Assets.AddAsync(asset);

    /// <inheritdoc />
    public Task Delete(Asset asset)
    {
        dbContext.Assets.Remove(asset);

        return Task.CompletedTask;
    }
}
