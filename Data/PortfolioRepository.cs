using Microsoft.EntityFrameworkCore;

namespace Portfoli.Data;

/// <summary>
/// Implementation of the <see cref="IPortfolioRepository"> interface using the Entity Framework database context.
/// </summary>
/// <param name="dbContext">The database context.</param>
public class PortfolioRepository(PortfoliDbContext dbContext) : IPortfolioRepository
{
    /// <inheritdoc />
    public async Task<Portfolio?> Get(PortfolioId id) => await GetQueryableWithDefaultIncludes().SingleOrDefaultAsync(p => p.Id == id);

    /// <inheritdoc />
    public IQueryable<Portfolio> GetAll() => GetQueryableWithDefaultIncludes();

    /// <inheritdoc />
    public async Task Add(Portfolio portfolio) => await dbContext.Portfolios.AddAsync(portfolio);

    /// <inheritdoc />
    public Task Delete(Portfolio portfolio)
    {
        dbContext.Portfolios.Remove(portfolio);

        return Task.CompletedTask;
    }

    private IQueryable<Portfolio> GetQueryableWithDefaultIncludes()
    {
        return dbContext.Portfolios
            .Include(p => p.Holdings)
            .ThenInclude(h => h.Asset)
            .Include(p => p.Holdings)
            .ThenInclude(h => h.Transactions);
    }
}
