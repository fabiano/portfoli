namespace Portfoli.Api.UseCases;

public static class DeleteTransaction
{
    public static IEndpointRouteBuilder MapDeleteTransactionEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints
            .MapDelete("/transactions/{transactionId:guid}", async (Guid transactionId, DeleteTransactionHandler handler) =>
            {
                var result = await handler.Handle(new DeleteTransactionRequest(transactionId));

                return result.Match(
                    onSuccess: () => Results.NoContent(),
                    onError: error => Results.Extensions.FromError(error));
            })
            .WithName(nameof(DeleteTransaction));

        return endpoints;
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
                return NewError(validationResult);
            }

            var transaction = await unitOfWork.Transactions.Get(request.TransactionId);

            if (transaction is null)
            {
                return NewItemNotFoundError($"Transaction {request.TransactionId} not found.");
            }

            await unitOfWork.Transactions.Delete(transaction);
            await unitOfWork.SaveChanges();

            return Result.Success;
        }
    }

    public record DeleteTransactionRequest(TransactionId TransactionId);

    public class DeleteTransactionRequestValidator : AbstractValidator<DeleteTransactionRequest>
    {
        public DeleteTransactionRequestValidator()
        {
            RuleFor(p => p.TransactionId)
                .NotEmpty();
        }
    }
}
