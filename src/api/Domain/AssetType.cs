namespace Portfoli.Domain;

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
