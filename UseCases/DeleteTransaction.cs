namespace Portfoli.UseCases;

public static class DeleteTransaction
{
    public static IEndpointRouteBuilder MapDeleteTransactionEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapDelete("/portfolios/{portfolioId:guid}/holdings/{holdingId:guid}/transactions/{transactionId:guid}", async (
            [FromRoute] PortfolioId portfolioId,
            [FromRoute] HoldingId holdingId,
            [FromRoute] TransactionId transactionId,
            [FromServices] IUnitOfWork unitOfWork) =>
        {
            var portfolio = await unitOfWork.Portfolios.Get(portfolioId);

            if (portfolio is null)
            {
                return Results.NotFound($"Portfolio {portfolioId} not found.");
            }

            var holding = portfolio.GetHolding(holdingId);

            if (holding is null)
            {
                return Results.NotFound($"Holding {holdingId} not found in portfolio {portfolioId}.");
            }

            var transaction = holding.GetTransaction(transactionId);

            if (transaction is null)
            {
                return Results.NotFound($"Transaction {transactionId} not found in holding {holdingId} of portfolio {portfolioId}.");
            }

            portfolio.RemoveTransaction(holding, transaction);

            await unitOfWork.SaveChanges();

            return Results.NoContent();
        });

        return endpoints;
    }

    public static IServiceCollection AddDeleteTransactionServices(this IServiceCollection services) => services;
}
