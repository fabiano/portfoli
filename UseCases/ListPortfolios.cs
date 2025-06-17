namespace Portfoli.UseCases;

public static class ListPortfolios
{
    public static IEndpointRouteBuilder MapListPortfoliosEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/portfolios", async ([FromServices] ReadingDbContext dbContext) =>
        {
            var response = await dbContext.Portfolios
                .OrderBy(p => p.Name)
                .Select(p => new ListPortfoliosResponse(p.Id, p.Name))
                .ToListAsync();

            return Results.Ok(response);
        });

        return endpoints;
    }

    public static IServiceCollection AddListPortfoliosServices(this IServiceCollection services) => services;

    public record ListPortfoliosResponse(PortfolioId Id, string Name);
}
