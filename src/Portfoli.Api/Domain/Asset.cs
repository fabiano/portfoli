namespace Portfoli.Api.Domain;

/// <summary>
/// Represents an asset that can be held in a portfolio.
/// </summary>
public class Asset
{
    /// <summary>
    /// The exchange where the asset is traded.
    /// </summary>
    public required string Exchange { get; init; }

    /// <summary>
    /// The ticker symbol of the asset.
    /// </summary>
    public required string Ticker { get; init; }

    /// <summary>
    /// The name of the asset.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The type of the asset (e.g., stock, ETF, crypto).
    /// </summary>
    public required AssetType Type { get; init; }
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
