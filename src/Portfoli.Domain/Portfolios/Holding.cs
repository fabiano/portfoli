namespace Portfoli.Domain.Portfolios;

/// <summary>
/// Represents a holding of an asset in a portfolio.
/// </summary>
public class Holding
{
    /// <summary>
    /// Unique identifier for the holding.
    /// </summary>
    public HoldingId Id { get; private set; } = Guid.NewGuid();

    /// <summary>
    /// Asset associated with the holding.
    /// </summary>
    public required Asset Asset { get; init; }

    /// <summary>
    /// The total quantity of the asset held.
    /// </summary>
    public decimal Quantity { get; private set; }
}
