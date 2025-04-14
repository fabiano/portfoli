namespace Portfoli.UseCases;

public static class GetPortfolioExtensions
{
    public static RouteGroupBuilder MapGetPortfolioEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/{portfolioId:guid}", async (Guid portfolioId, GetPortfolioHandler handler) =>
        {
            var result = await handler.Handle(new GetPortfolioRequest(portfolioId));

            return Results.Ok(result);
        });

        return group;
    }

    public static IServiceCollection AddGetPortfolioServices(this IServiceCollection services)
    {
        services.AddScoped<GetPortfolioHandler>();

        return services;
    }
}

public class GetPortfolioHandler(IUnitOfWork unitOfWork)
{
    public async Task<GetPortfolioResponse> Handle(GetPortfolioRequest request)
    {
        var portfolio = await unitOfWork.Portfolios.Get(request.Id) ?? throw new PortfolioNotFoundException(request.Id);

        return new GetPortfolioResponse(portfolio.Id, portfolio.Name);
    }
}

public record GetPortfolioRequest(PortfolioId Id);

public record GetPortfolioResponse(PortfolioId Id, string Name);
