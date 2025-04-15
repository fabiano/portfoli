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

        return services;
    }

    public class GetPortfolioHandler(IUnitOfWork unitOfWork)
    {
        public async Task<Result<GetPortfolioResponse>> Handle(GetPortfolioRequest request)
        {
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
}
