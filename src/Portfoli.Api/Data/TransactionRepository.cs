using Microsoft.EntityFrameworkCore;

namespace Portfoli.Api.Data;

/// <summary>
/// Implementation of the <see cref="ITransactionRepository"/> interface using the Entity Framework database context.
/// </summary>
/// <param name="dbContext">The database context.</param>
public class TransactionRepository(WritingDbContext dbContext) : ITransactionRepository
{
    /// <inheritdoc />
    public async Task<Transaction?> Get(TransactionId id) => await dbContext.Transactions.SingleOrDefaultAsync(t => t.Id == id);

    /// <inheritdoc />
    public async Task Add(Transaction transaction) => await dbContext.Transactions.AddAsync(transaction);

    /// <inheritdoc />
    public Task Delete(Transaction transaction)
    {
        dbContext.Transactions.Remove(transaction);

        return Task.CompletedTask;
    }
}
