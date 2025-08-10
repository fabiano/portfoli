namespace Portfoli.Api.Endpoints;

public static class DeleteHolding
{
    public static IEndpointRouteBuilder MapDeleteHoldingEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints
            .MapDelete("/holdings/{holdingId:guid}", async (Guid holdingId, DeleteHoldingHandler handler) =>
            {
                var result = await handler.Handle(new DeleteHoldingRequest(holdingId));

                return result.Match(
                    onSuccess: () => Results.NoContent(),
                    onError: error => Results.Extensions.FromError(error));
            })
            .WithName(nameof(DeleteHolding));

        return endpoints;
    }

    public static IServiceCollection AddDeleteHoldingServices(this IServiceCollection services)
    {
        services.AddScoped<DeleteHoldingHandler>();
        services.AddScoped<DeleteHoldingRequestValidator>();

        return services;
    }

    public class DeleteHoldingHandler(IUnitOfWork unitOfWork, DeleteHoldingRequestValidator validator)
    {
        public async Task<Result> Handle(DeleteHoldingRequest request)
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return NewError(validationResult);
            }

            var portfolio = await unitOfWork.Portfolios.GetByHolding(request.HoldingId);

            if (portfolio is null)
            {
                return NewItemNotFoundError($"Holding {request.HoldingId} not found.");
            }

            var holding = portfolio.GetHolding(request.HoldingId);

            if (holding is null)
            {
                return NewItemNotFoundError($"Holding {request.HoldingId} not found.");
            }

            portfolio.RemoveHolding(holding);

            await unitOfWork.SaveChanges();

            return Result.Success;
        }
    }

    public record DeleteHoldingRequest(HoldingId HoldingId);

    public class DeleteHoldingRequestValidator : AbstractValidator<DeleteHoldingRequest>
    {
        public DeleteHoldingRequestValidator()
        {
            RuleFor(p => p.HoldingId)
                .NotEmpty();
        }
    }
}
