using Microsoft.EntityFrameworkCore;

namespace Portfoli.UseCases;

public static class ListPortfolios
{
    public static RouteGroupBuilder MapListPortfoliosEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (ListPortfoliosHandler handler) =>
        {
            var result = await handler.Handle(new ListPortfoliosRequest());

            return result.Match(
                onSuccess: response => Results.Ok(response),
                onError: error => Results.Extensions.FromError(error));
        });

        return group;
    }

    public static IServiceCollection AddListPortfoliosServices(this IServiceCollection services)
    {
        services.AddScoped<ListPortfoliosHandler>();

        return services;
    }

    public class ListPortfoliosHandler(PortfoliDbContext dbContext)
    {
        public async Task<Result<IEnumerable<ListPortfoliosResponse>>> Handle(ListPortfoliosRequest request) => await dbContext.Portfolios
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .Select(p => new ListPortfoliosResponse(p.Id, p.Name))
            .ToListAsync();
    }

    public record ListPortfoliosRequest;

    public record ListPortfoliosResponse(PortfolioId Id, string Name);
}
