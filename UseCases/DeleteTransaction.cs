namespace Portfoli.UseCases;

public static class DeleteTransaction
{
    public static RouteGroupBuilder MapDeleteTransactionEndpoint(this RouteGroupBuilder group)
    {
        group.MapDelete("/{transactionId:guid}", async (Guid transactionId, Guid portfolioId, Guid holdingId, DeleteTransactionHandler handler) =>
        {
            var result = await handler.Handle(new DeleteTransactionRequest(portfolioId, holdingId, transactionId));

            return result.Match(
                onSuccess: () => Results.NoContent(),
                onError: error => Results.Extensions.FromError(error));
        });

        return group;
    }

    public static IServiceCollection AddDeleteTransactionServices(this IServiceCollection services)
    {
        services.AddScoped<DeleteTransactionHandler>();
        services.AddScoped<DeleteTransactionRequestValidator>();

        return services;
    }

    public class DeleteTransactionHandler(IUnitOfWork unitOfWork, DeleteTransactionRequestValidator validator)
    {
        public async Task<Result> Handle(DeleteTransactionRequest request)
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return Error.New(validationResult);
            }

            var portfolio = await unitOfWork.Portfolios.Get(request.PortfolioId);

            if (portfolio is null)
            {
                return Error.NotFound($"Portfolio {request.PortfolioId} not found.");
            }

            var holding = portfolio.GetHolding(request.HoldingId);

            if (holding is null)
            {
                return Error.NotFound($"Holding {request.HoldingId} not found in portfolio {request.PortfolioId}.");
            }

            var transaction = holding.GetTransaction(request.TransactionId);

            if (transaction is null)
            {
                return Error.NotFound($"Transaction {request.TransactionId} not found in holding {request.HoldingId} of portfolio {request.PortfolioId}.");
            }

            portfolio.RemoveTransaction(holding, transaction);

            await unitOfWork.SaveChanges();

            return Result.Success;
        }
    }

    public record DeleteTransactionRequest(PortfolioId PortfolioId, HoldingId HoldingId, TransactionId TransactionId);

    public class DeleteTransactionRequestValidator : AbstractValidator<DeleteTransactionRequest>
    {
        public DeleteTransactionRequestValidator()
        {
            RuleFor(p => p.PortfolioId)
                .NotEmpty();

            RuleFor(p => p.HoldingId)
                .NotEmpty();

            RuleFor(p => p.TransactionId)
                .NotEmpty();
        }
    }
}
