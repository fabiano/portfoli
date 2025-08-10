using System.Text.Json;
using System.Text.Json.Serialization;

namespace Portfoli.Api.Domain;

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
