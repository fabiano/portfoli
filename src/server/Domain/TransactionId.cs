namespace Portfoli.Domain;

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
}

/// <summary>
/// Json converter for the TransactionId type.
/// </summary>
public class TransactionIdJsonConverter : JsonConverter<TransactionId>
{
    public override TransactionId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => new(reader.GetGuid());

    public override void Write(Utf8JsonWriter writer, TransactionId value, JsonSerializerOptions options) => writer.WriteStringValue(value.Value.ToString());
}
