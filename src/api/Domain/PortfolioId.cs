namespace Portfoli.Domain;

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
}

/// <summary>
/// Json converter for the PortfolioId type.
/// </summary>
public class PortfolioIdJsonConverter : JsonConverter<PortfolioId>
{
    public override PortfolioId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => new(reader.GetGuid());

    public override void Write(Utf8JsonWriter writer, PortfolioId value, JsonSerializerOptions options) => writer.WriteStringValue(value.Value.ToString());
}
