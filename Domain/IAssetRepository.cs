namespace Portfoli.Domain;

/// <summary>
/// Repository for managing assets in the portfolio database.
/// </summary>
public interface IAssetRepository
{
    /// <summary>
    /// Retrieves an asset by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the asset.</param>
    /// <returns>A task representing the asynchronous operation, with the asset as the result.</returns>
    Task<Asset?> Get(AssetId id);

    /// <summary>
    /// Retrieves an asset by its exchange and ticker symbol.
    /// </summary>
    /// <param name="exchange">The exchange where the asset is traded.</param>
    /// <param name="ticker">The ticker symbol of the asset.</param>
    /// <returns>A task representing the asynchronous operation, with the asset as the result.</returns>
    Task<Asset?> GetByTicker(string exchange, string ticker);

    /// <summary>
    /// Retrieves all assets in the repository.
    /// </summary>
    /// <returns>An IQueryable collection of assets.</returns>
    IQueryable<Asset> GetAll();

    /// <summary>
    /// Adds a new asset to the repository.
    /// </summary>
    /// <param name="Asset">The asset to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task Add(Asset Asset);

    /// <summary>
    /// Updates an existing asset in the repository.
    /// </summary>
    /// <param name="Asset">The asset to update.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task Delete(Asset Asset);
}
