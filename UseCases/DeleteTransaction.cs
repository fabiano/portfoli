namespace Portfoli.UseCases;

public static class DeleteTransactionExtensions
{
    public static RouteGroupBuilder MapDeleteTransactionEndpoint(this RouteGroupBuilder group)
    {
        group.MapDelete("/{transactionId:guid}", async (Guid transactionId, Guid portfolioId, Guid holdingId, DeleteTransactionHandler handler) =>
        {
            await handler.Handle(new DeleteTransactionRequest(portfolioId, holdingId, transactionId));

            return Results.NoContent();
        });

        return group;
    }

    public static IServiceCollection AddDeleteTransactionServices(this IServiceCollection services)
    {
        services.AddScoped<DeleteTransactionHandler>();

        return services;
    }
}

public class DeleteTransactionHandler(IUnitOfWork unitOfWork)
{
    public async Task Handle(DeleteTransactionRequest request)
    {
        var portfolio = await unitOfWork.Portfolios.Get(request.PortfolioId) ?? throw new PortfolioNotFoundException(request.PortfolioId);
        var holding = portfolio.GetHolding(request.HoldingId) ?? throw new HoldingNotFoundException(request.PortfolioId, request.HoldingId);
        var transaction = holding.GetTransaction(request.TransactionId) ?? throw new TransactionNotFoundException(request.PortfolioId, request.HoldingId, request.TransactionId);

        portfolio.RemoveTransaction(holding, transaction);

        await unitOfWork.SaveChanges();
    }
}

public record DeleteTransactionRequest(PortfolioId PortfolioId, HoldingId HoldingId, TransactionId TransactionId);
