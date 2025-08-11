namespace Portfoli.Domain.Portfolios;

/// <summary>
/// Represents a portfolio of assets.
/// </summary>
public class Portfolio
{
    private readonly List<Holding> holdings = [];

    /// <summary>
    /// Unique identifier for the portfolio.
    /// </summary>
    public PortfolioId Id { get; private set; } = Guid.NewGuid();

    /// <summary>
    /// Name of the portfolio.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Holdings in the portfolio.
    /// </summary>
    public IReadOnlyCollection<Holding> Holdings => holdings.AsReadOnly();

    /// <summary>
    /// Adds a new holding to the portfolio.
    /// </summary>
    /// <param name="holding">The holding to add.</param>
    public void AddHolding(Holding holding)
    {
        if (holding is null)
        {
            throw new ArgumentNullException(nameof(holding), "Holding cannot be null.");
        }

        if (holdings.Contains(holding))
        {
            throw new InvalidDomainOperationException($"Holding {holding.Id} already exists in this portfolio.");
        }

        if (holdings.Any(h => holding.Asset.Ticker == holding.Asset.Ticker && h.Asset.Exchange == holding.Asset.Exchange))
        {
            throw new InvalidDomainOperationException($"Holding for {holding.Asset.Ticker} on {holding.Asset.Exchange} already exists.");
        }

        holdings.Add(holding);
    }

    /// <summary>
    /// Removes a holding from the portfolio.
    /// </summary>
    /// <param name="holdingToRemove">The holding to remove.</param>
    public void RemoveHolding(Holding holdingToRemove)
    {
        if (holdingToRemove is null)
        {
            throw new ArgumentNullException(nameof(holdingToRemove), "Holding cannot be null.");
        }

        if (!holdings.Contains(holdingToRemove))
        {
            throw new InvalidDomainOperationException($"Holding {holdingToRemove.Id} not found in this portfolio.");
        }

        holdings.Remove(holdingToRemove);
    }

    /// <summary>
    /// Gets a holding by its unique identifier.
    /// </summary>
    /// <param name="holdingId">The unique identifier of the holding.</param>
    /// <returns>The holding if found; otherwise, null.</returns>
    public Holding? GetHolding(HoldingId holdingId) => holdings.SingleOrDefault(h => h.Id == holdingId);
}
