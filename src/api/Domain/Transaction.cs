namespace Portfoli.Domain;

/// <summary>
/// Represents a transaction of an asset in a portfolio.
/// </summary>
public class Transaction
{
    /// <summary>
    /// Unique identifier for the transaction.
    /// </summary>
    public TransactionId Id { get; private set; } = Guid.NewGuid();

    /// <summary>
    /// Portfolio to which the transaction belongs.
    /// </summary>
    public required PortfolioId PortfolioId { get; init; }

    /// <summary>
    /// Holding associated with the transaction.
    /// </summary>
    public required HoldingId HoldingId { get; init; }

    /// <summary>
    /// Type of the transaction (buy/sell).
    /// </summary>
    public required TransactionType Type { get; set; }

    /// <summary>
    /// Date and time of the transaction.
    /// </summary>
    public required DateTimeOffset Date { get; set; }

    /// <summary>
    /// Quantity of the asset involved in the transaction.
    /// </summary>
    public required decimal Quantity { get; set; }

    /// <summary>
    /// Price per unit of the asset at the time of the transaction.
    /// </summary>
    public required decimal Price { get; set; }

    /// <summary>
    /// Commission or fees associated with the transaction.
    /// </summary>
    public decimal Commission { get; set; }
}

/// <summary>
/// Represents the type of transaction (buy or sell).
/// </summary>
public enum TransactionType
{
    /// <summary>
    /// Represents a buy transaction.
    /// </summary>
    Buy,

    /// <summary>
    /// Represents a sell transaction.
    /// </summary>
    Sell,
}
