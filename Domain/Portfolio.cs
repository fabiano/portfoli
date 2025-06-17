using System.Text.Json;
using System.Text.Json.Serialization;

namespace Portfoli.Domain;

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

    /// <summary>
    /// Adds a transaction to a holding in the portfolio.
    /// </summary>
    /// <param name="holding">The holding to which the transaction will be added.</param>
    /// <param name="transaction">The transaction to add.</param>
    public void AddTransaction(Holding holding, Transaction transaction)
    {
        if (holding is null)
        {
            throw new ArgumentNullException(nameof(holding), "Holding cannot be null.");
        }

        if (transaction is null)
        {
            throw new ArgumentNullException(nameof(transaction), "Transaction cannot be null.");
        }

        holding.AddTransaction(transaction);
    }

    /// <summary>
    /// Removes a transaction from a holding in the portfolio.
    /// </summary>
    /// <param name="holding">The holding from which the transaction will be removed.</param>
    /// <param name="transactionToRemove">The transaction to remove.</param>
    public void RemoveTransaction(Holding holding, Transaction transactionToRemove)
    {
        if (holding is null)
        {
            throw new ArgumentNullException(nameof(holding), "Holding cannot be null.");
        }

        if (transactionToRemove is null)
        {
            throw new ArgumentNullException(nameof(transactionToRemove), "Transaction cannot be null.");
        }

        holding.RemoveTransaction(transactionToRemove);
    }

    /// <summary>
    /// Gets a transaction by its unique identifier from a specific holding in the portfolio.
    /// </summary>
    /// <param name="holdingId">The unique identifier of the holding.</param>
    /// <param name="transactionId">The unique identifier of the transaction.</param>
    /// <returns>The transaction if found; otherwise, null.</returns>
    public Transaction? GetTransaction(HoldingId holdingId, TransactionId transactionId)
    {
        var holding = GetHolding(holdingId);

        return holding?.GetTransaction(transactionId);
    }
}

/// <summary>
/// Represents a unique identifier for a portfolio.
/// </summary>
/// <param name="Value">The underlying GUID value of the portfolio identifier.</param>
[JsonConverter(typeof(PortfolioIdJsonConverter))]
public record PortfolioId(Guid Value)
{
    /// <summary>
    /// Implicitly converts a PortfolioId to a Guid.
    /// </summary>
    /// <param name="portfolioId">The PortfolioId to convert.</param>
    public static implicit operator Guid(PortfolioId portfolioId) => portfolioId.Value;

    /// <summary>
    /// Implicitly converts a Guid to a PortfolioId.
    /// </summary>
    /// <param name="value">The Guid value to convert.</param>
    public static implicit operator PortfolioId(Guid value) => new(value);

    /// <summary>
    /// Returns a string representation of the PortfolioId.
    /// </summary>
    /// <returns>A string representation of the PortfolioId.</returns>
    override public string ToString() => Value.ToString();

    /// <summary>
    /// Converts a string representation of a PortfolioId to its corresponding PortfolioId object.
    /// </summary>
    /// <param name="value">The string representation of the PortfolioId.</param>
    /// <param name="portfolioId">The resulting PortfolioId object.</param>
    /// <returns>true if the conversion was successful; otherwise, false.</returns>
    public static bool TryParse(string value, out PortfolioId portfolioId)
    {
        if (Guid.TryParse(value, out var guid))
        {
            portfolioId = guid;

            return true;
        }

        portfolioId = default!;

        return false;
    }
}

/// <summary>
/// Json converter for the PortfolioId type.
/// </summary>
public class PortfolioIdJsonConverter : JsonConverter<PortfolioId>
{
    public override PortfolioId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => new(reader.GetGuid());

    public override void Write(Utf8JsonWriter writer, PortfolioId value, JsonSerializerOptions options) => writer.WriteStringValue(value.Value.ToString());
}
