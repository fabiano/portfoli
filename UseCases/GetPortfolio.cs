namespace Portfoli.UseCases;

public static class GetPortfolio
{
    public static RouteGroupBuilder MapGetPortfolioEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/{portfolioId:guid}", async (Guid portfolioId, GetPortfolioHandler handler) =>
        {
            var result = await handler.Handle(new GetPortfolioRequest(portfolioId));

            return result.Match(
                onSuccess: response => Results.Ok(response),
                onError: error => Results.Extensions.FromError(error));
        });

        return group;
    }

    public static IServiceCollection AddGetPortfolioServices(this IServiceCollection services)
    {
        services.AddScoped<GetPortfolioHandler>();
        services.AddScoped<GetPortfolioRequestValidator>();

        return services;
    }

    public class GetPortfolioHandler(IUnitOfWork unitOfWork, GetPortfolioRequestValidator validator)
    {
        public async Task<Result<GetPortfolioResponse>> Handle(GetPortfolioRequest request)
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return Error.New(validationResult);
            }

            var portfolio = await unitOfWork.Portfolios.Get(request.Id);

            if (portfolio is null)
            {
                return Error.NotFound($"Portfolio {request.Id} not found.");
            }

            return new GetPortfolioResponse(portfolio.Id, portfolio.Name);
        }
    }

    public record GetPortfolioRequest(PortfolioId Id);

    public record GetPortfolioResponse(PortfolioId Id, string Name);

    public class GetPortfolioRequestValidator : AbstractValidator<GetPortfolioRequest>
    {
        public GetPortfolioRequestValidator()
        {
            RuleFor(p => p.Id)
                .NotEmpty();
        }
    }
}
