namespace Portfoli.Domain;

/// <summary>
/// Interface for the transaction repository, providing methods to manage transactions.
/// </summary>
public interface ITransactionRepository
{
    /// <summary>
    /// Gets a transaction by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the transaction.</param>
    /// <returns>A task representing the asynchronous operation, with the transaction as the result.</returns>
    Task<Transaction?> Get(TransactionId id);

    /// <summary>
    /// Adds a new transaction to the repository.
    /// </summary>
    /// <param name="transaction">The transaction to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task Add(Transaction transaction);

    /// <summary>
    /// Deletes an existing transaction from the repository.
    /// </summary>
    /// <param name="transaction">The transaction to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task Delete(Transaction transaction);
}
