using Microsoft.EntityFrameworkCore;

namespace Portfoli.Data;

/// <summary>
/// Implementation of the <see cref="IPortfolioRepository"> interface using the Entity Framework database context.
/// </summary>
/// <param name="dbContext">The database context.</param>
public class PortfolioRepository(WritingDbContext dbContext) : IPortfolioRepository
{
    /// <inheritdoc />
    public async Task<Portfolio?> Get(PortfolioId id) => await dbContext.Portfolios
        .Include(p => p.Holdings)
        .SingleOrDefaultAsync(p => p.Id == id);

    /// <inheritdoc />
    public async Task<Portfolio?> GetByHolding(HoldingId holdingId) => await dbContext.Portfolios
        .Include(p => p.Holdings)
        .SingleOrDefaultAsync(p => p.Holdings.Any(h => h.Id == holdingId));

    /// <inheritdoc />
    public async Task Add(Portfolio portfolio) => await dbContext.Portfolios.AddAsync(portfolio);

    /// <inheritdoc />
    public Task Delete(Portfolio portfolio)
    {
        dbContext.Portfolios.Remove(portfolio);

        return Task.CompletedTask;
    }
}
