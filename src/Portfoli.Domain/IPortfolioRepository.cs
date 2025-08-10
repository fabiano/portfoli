namespace Portfoli.Domain;

/// <summary>
/// Interface for the portfolio repository, providing methods to manage portfolios and their holdings.
/// </summary>
public interface IPortfolioRepository
{
    /// <summary>
    /// Gets a portfolio by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the portfolio.</param>
    /// <returns>A task representing the asynchronous operation, with the portfolio as the result.</returns>
    Task<Portfolio?> Get(PortfolioId id);

    /// <summary>
    /// Gets a portfolio by a holding's unique identifier.
    /// </summary>
    /// <param name="holdingId">The unique identifier of the holding.</param>
    /// <returns>A task representing the asynchronous operation, with the portfolio containing the holding as the result.</returns>
    Task<Portfolio?> GetByHolding(HoldingId holdingId);

    /// <summary>
    /// Adds a new portfolio to the repository.
    /// </summary>
    /// <param name="portfolio">The portfolio to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task Add(Portfolio portfolio);

    /// <summary>
    /// Deletes an existing portfolio in the repository.
    /// </summary>
    /// <param name="portfolio">The portfolio to update.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task Delete(Portfolio portfolio);
}
