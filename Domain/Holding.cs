using System.Text.Json;
using System.Text.Json.Serialization;

namespace Portfoli.Domain;

/// <summary>
/// Represents a holding of an asset in a portfolio.
/// </summary>
public class Holding
{
    private readonly List<Transaction> transactions = [];

    /// <summary>
    /// Unique identifier for the holding.
    /// </summary>
    public HoldingId Id { get; private set; } = Guid.NewGuid();

    /// <summary>
    /// Asset associated with the holding.
    /// </summary>
    public required Asset Asset { get; set; }

    /// <summary>
    /// The total quantity of the asset held.
    /// </summary>
    public decimal Quantity { get; private set; }

    /// <summary>
    /// The transaction history for the holding.
    /// </summary>
    public IReadOnlyCollection<Transaction> Transactions => transactions.AsReadOnly();

    /// <summary>
    /// Adds a transaction to the holding.
    /// </summary>
    /// <param name="transaction">The transaction to add.</param>
    public void AddTransaction(Transaction transaction)
    {
        if (transaction is null)
        {
            throw new ArgumentNullException(nameof(transaction), "Transaction cannot be null.");
        }

        if (transactions.Contains(transaction))
        {
            throw new InvalidDomainOperationException($"Transaction {transaction.Id} already exists in this holding.");
        }

        Quantity = transaction.Type == TransactionType.Buy
            ? Quantity + transaction.Quantity
            : Quantity - transaction.Quantity;

        if (Quantity < 0)
        {
            throw new InvalidDomainOperationException($"Adding transaction {transaction.Id} would result in negative quantity for this holding.");
        }

        transactions.Add(transaction);
    }

    /// <summary>
    /// Removes a transaction from the holding.
    /// </summary>
    /// <param name="transactionToRemove">The transaction to remove.</param>
    public void RemoveTransaction(Transaction transactionToRemove)
    {
        if (transactionToRemove is null)
        {
            throw new ArgumentNullException(nameof(transactionToRemove), "Transaction cannot be null.");
        }

        if (!transactions.Contains(transactionToRemove))
        {
            throw new InvalidDomainOperationException($"Transaction {transactionToRemove.Id} not found in this holding.");
        }

        Quantity = transactionToRemove.Type == TransactionType.Buy
            ? Quantity - transactionToRemove.Quantity
            : Quantity + transactionToRemove.Quantity;

        transactions.Remove(transactionToRemove);
    }

    /// <summary>
    /// Gets a transaction by its unique identifier.
    /// </summary>
    /// <param name="transactionId">The unique identifier of the transaction.</param>
    /// <returns>The transaction if found; otherwise, null.</returns>
    public Transaction? GetTransaction(TransactionId transactionId) => transactions.SingleOrDefault(t => t.Id == transactionId);
}

/// <summary>
/// Represents a unique identifier for a holding.
/// </summary>
/// <param name="Value">The underlying GUID value of the holding identifier.</param>
[JsonConverter(typeof(HoldingIdJsonConverter))]
public record HoldingId(Guid Value)
{
    /// <summary>
    /// Implicitly converts a HoldingId to a Guid.
    /// </summary>
    /// <param name="holdingId">The HoldingId to convert.</param>
    public static implicit operator Guid(HoldingId holdingId) => holdingId.Value;

    /// <summary>
    /// Implicitly converts a Guid to a HoldingId.
    /// </summary>
    /// <param name="value">The Guid value to convert.</param>
    public static implicit operator HoldingId(Guid value) => new(value);

    /// <summary>
    /// Returns a string representation of the HoldingId.
    /// </summary>
    /// <returns>A string representation of the HoldingId.</returns>
    override public string ToString() => Value.ToString();
}

/// <summary>
/// Json converter for the HoldingId type.
/// </summary>
public class HoldingIdJsonConverter : JsonConverter<HoldingId>
{
    public override HoldingId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => new(reader.GetGuid());

    public override void Write(Utf8JsonWriter writer, HoldingId value, JsonSerializerOptions options) => writer.WriteStringValue(value.Value.ToString());
}
