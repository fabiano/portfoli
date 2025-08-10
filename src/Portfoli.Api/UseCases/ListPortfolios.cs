using Microsoft.EntityFrameworkCore;

namespace Portfoli.Api.UseCases;

public static class GetPortfolios
{
    public static IEndpointRouteBuilder MapListPortfoliosEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints
            .MapGet("/portfolios", async (GetPortfoliosHandler handler) =>
            {
                var result = await handler.Handle(new GetPortfoliosRequest());

                return result.Match(
                    onSuccess: response => Results.Ok(response),
                    onError: error => Results.Extensions.FromError(error));
            })
            .WithName(nameof(GetPortfolios));

        return endpoints;
    }

    public static IServiceCollection AddListPortfoliosServices(this IServiceCollection services)
    {
        services.AddScoped<GetPortfoliosHandler>();

        return services;
    }

    public class GetPortfoliosHandler(ReadingDbContext dbContext)
    {
        public async Task<Result<IEnumerable<GetPortfoliosResponse>>> Handle(GetPortfoliosRequest request) => await dbContext.Portfolios
            .OrderBy(p => p.Name)
            .Select(p => new GetPortfoliosResponse(p.Id, p.Name))
            .ToListAsync();
    }

    public record GetPortfoliosRequest;

    public record GetPortfoliosResponse(PortfolioId Id, string Name);
}
