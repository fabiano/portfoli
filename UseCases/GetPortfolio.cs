namespace Portfoli.UseCases;

public static class GetPortfolio
{
    public static IEndpointRouteBuilder MapGetPortfolioEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/portfolios/{portfolioId:guid}", async (
            [FromRoute] PortfolioId portfolioId,
            [FromServices] ReadingDbContext dbContext) =>
        {
            var response = await dbContext.Portfolios
                .Where(p => p.Id == portfolioId)
                .Select(p => new GetPortfolioResponse(p.Id, p.Name))
                .SingleOrDefaultAsync();

            if (response is null)
            {
                return Results.NotFound($"Portfolio {portfolioId} not found.");
            }

            return Results.Ok(response);
        });

        return endpoints;
    }

    public static IServiceCollection AddGetPortfolioServices(this IServiceCollection services) => services;

    public record GetPortfolioResponse(PortfolioId Id, string Name);
}
