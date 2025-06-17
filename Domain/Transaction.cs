using System.Text.Json;
using System.Text.Json.Serialization;

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
/// Represents a unique identifier for a transaction.
/// </summary>
/// <param name="Value">The underlying GUID value of the transaction identifier.</param>
[JsonConverter(typeof(TransactionIdJsonConverter))]
public record TransactionId(Guid Value)
{
    /// <summary>
    /// Implicitly converts a TransactionId to a Guid.
    /// </summary>
    /// <param name="transactionId">The TransactionId to convert.</param>
    public static implicit operator Guid(TransactionId transactionId) => transactionId.Value;

    /// <summary>
    /// Implicitly converts a Guid to a TransactionId.
    /// </summary>
    /// <param name="value">The Guid value to convert.</param>
    public static implicit operator TransactionId(Guid value) => new(value);

    /// <summary>
    /// Returns a string representation of the TransactionId.
    /// </summary>
    /// <returns>A string representation of the TransactionId.</returns>
    override public string ToString() => Value.ToString();

    /// <summary>
    /// Converts a string representation of a TransactionId to its corresponding TransactionId object.
    /// </summary>
    /// <param name="value">The string representation of the TransactionId.</param>
    /// <param name="transactionId">The resulting TransactionId object.</param>
    /// <returns>true if the conversion was successful; otherwise, false.</returns>
    public static bool TryParse(string value, out TransactionId transactionId)
    {
        if (Guid.TryParse(value, out var guid))
        {
            transactionId = guid;

            return true;
        }

        transactionId = default!;

        return false;
    }
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
    Sell
}

/// <summary>
/// Json converter for the TransactionId type.
/// </summary>
public class TransactionIdJsonConverter : JsonConverter<TransactionId>
{
    public override TransactionId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => new(reader.GetGuid());

    public override void Write(Utf8JsonWriter writer, TransactionId value, JsonSerializerOptions options) => writer.WriteStringValue(value.Value.ToString());
}
