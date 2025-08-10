namespace Portfoli.Api.UseCases;

public static class DeletePortfolio
{
    public static IEndpointRouteBuilder MapDeletePortfolioEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints
            .MapDelete("/portfolios/{portfolioId:guid}", async (Guid portfolioId, DeletePortfolioHandler handler) =>
            {
                var result = await handler.Handle(new DeletePortfolioRequest(portfolioId));

                return result.Match(
                    onSuccess: () => Results.NoContent(),
                    onError: error => Results.Extensions.FromError(error));
            })
            .WithName(nameof(DeletePortfolio));

        return endpoints;
    }

    public static IServiceCollection AddDeletePortfolioServices(this IServiceCollection services)
    {
        services.AddScoped<DeletePortfolioHandler>();
        services.AddScoped<DeletePortfolioRequestValidator>();

        return services;
    }

    public class DeletePortfolioHandler(IUnitOfWork unitOfWork, DeletePortfolioRequestValidator validator)
    {
        public async Task<Result> Handle(DeletePortfolioRequest request)
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return NewError(validationResult);
            }

            var portfolio = await unitOfWork.Portfolios.Get(request.Id);

            if (portfolio is null)
            {
                return NewItemNotFoundError($"Portfolio {request.Id} not found.");
            }

            await unitOfWork.Portfolios.Delete(portfolio);
            await unitOfWork.SaveChanges();

            return Result.Success;
        }
    }

    public record DeletePortfolioRequest(PortfolioId Id);

    public class DeletePortfolioRequestValidator : AbstractValidator<DeletePortfolioRequest>
    {
        public DeletePortfolioRequestValidator()
        {
            RuleFor(p => p.Id)
                .NotEmpty();
        }
    }
}
