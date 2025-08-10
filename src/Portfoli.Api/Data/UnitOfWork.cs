namespace Portfoli.Api.Data;

/// <summary>
/// Unit of Work pattern interface.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Gets the portfolio repository.
    /// </summary>
    IPortfolioRepository Portfolios { get; }

    /// <summary>
    /// Gets the transaction repository.
    /// </summary>
    ITransactionRepository Transactions { get; }

    /// <summary>
    /// Saves changes to the database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SaveChanges(CancellationToken cancellationToken = default);
}

/// <summary>
/// Implementation of the <see cref="IUnitOfWork"/> interface using the Entity Framework database context.
/// </summary>
/// <param name="serviceProvider">The service provider.</param>
/// <param name="dbContext">The database context.</param>
public class UnitOfWork(IServiceProvider serviceProvider, WritingDbContext dbContext) : IUnitOfWork
{
    /// <inheritdoc/>
    public IPortfolioRepository Portfolios => serviceProvider.GetRequiredService<IPortfolioRepository>();

    /// <inheritdoc/>
    public ITransactionRepository Transactions => serviceProvider.GetRequiredService<ITransactionRepository>();

    /// <inheritdoc/>
    public async Task SaveChanges(CancellationToken cancellationToken = default) => await dbContext.SaveChangesAsync(cancellationToken);
}
