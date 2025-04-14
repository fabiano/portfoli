using System.Text.Json;
using System.Text.Json.Serialization;

namespace Portfoli.Domain;

/// <summary>
/// Represents an asset that can be held in a portfolio.
/// </summary>
public class Asset
{
    /// <summary>
    /// Unique identifier for the asset.
    /// </summary>
    public AssetId Id { get; private set; } = Guid.NewGuid();

    /// <summary>
    /// The exchange where the asset is traded.
    /// </summary>
    public required string Exchange { get; set; }

    /// <summary>
    /// The ticker symbol of the asset.
    /// </summary>
    public required string Ticker { get; set; }

    /// <summary>
    /// The name of the asset.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// The type of the asset (e.g., stock, ETF, crypto).
    /// </summary>
    public required AssetType AssetType { get; set; }
}

/// <summary>
/// Represents a unique identifier for an asset.
/// This is a wrapper around a GUID to provide type safety and clarity in the code.
/// </summary>
/// <param name="Value">The underlying GUID value of the asset identifier.</param>
[JsonConverter(typeof(AssetIdJsonConverter))]
public record AssetId(Guid Value)
{
    /// <summary>
    /// Implicitly converts an AssetId to a Guid.
    /// </summary>
    /// <param name="assetId">The AssetId to convert.</param>
    public static implicit operator Guid(AssetId assetId) => assetId.Value;

    /// <summary>
    /// Implicitly converts a Guid to an AssetId.
    /// </summary>
    /// <param name="value">The Guid value to convert.</param>
    public static implicit operator AssetId(Guid value) => new(value);
}

/// <summary>
/// Represents the type of asset (e.g., stock, ETF, crypto).
/// </summary>
public enum AssetType
{
    /// <summary>
    /// Represents a stock asset.
    /// </summary>
    Stock,

    /// <summary>
    /// Represents an exchange-traded fund (ETF) asset.
    /// </summary>
    ETF,

    /// <summary>
    /// Represents a cryptocurrency asset.
    /// </summary>
    Crypto,
}

/// <summary>
/// Json converter for the AssetId type.
/// </summary>
public class AssetIdJsonConverter : JsonConverter<AssetId>
{
    public override AssetId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => new(reader.GetGuid());

    public override void Write(Utf8JsonWriter writer, AssetId value, JsonSerializerOptions options) => writer.WriteStringValue(value.Value.ToString());
}
