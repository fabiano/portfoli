namespace Portfoli.UseCases;

public static class DeleteHolding
{
    public static RouteGroupBuilder MapDeleteHoldingEndpoint(this RouteGroupBuilder group)
    {
        group.MapDelete("/{holdingId:guid}", async (Guid portfolioId, Guid holdingId, DeleteHoldingHandler handler) =>
        {
            var result = await handler.Handle(new DeleteHoldingRequest(portfolioId, holdingId));

            return result.Match(
                onSuccess: () => Results.NoContent(),
                onError: error => Results.Extensions.FromError(error));
        });

        return group;
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

            portfolio.RemoveHolding(holding);

            await unitOfWork.SaveChanges();

            return Result.Success;
        }
    }

    public record DeleteHoldingRequest(PortfolioId PortfolioId, HoldingId HoldingId);

    public class DeleteHoldingRequestValidator : AbstractValidator<DeleteHoldingRequest>
    {
        public DeleteHoldingRequestValidator()
        {
            RuleFor(p => p.PortfolioId)
                .NotEmpty();

            RuleFor(p => p.HoldingId)
                .NotEmpty();
        }
    }
}
