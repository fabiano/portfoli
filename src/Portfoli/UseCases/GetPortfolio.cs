using Microsoft.EntityFrameworkCore;

namespace Portfoli.UseCases;

public static class GetPortfolio
{
    public static IEndpointRouteBuilder MapGetPortfolioEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints
            .MapGet("/portfolios/{portfolioId:guid}", async (Guid portfolioId, GetPortfolioHandler handler) =>
            {
                var result = await handler.Handle(new GetPortfolioRequest(portfolioId));

                return result.Match(
                    onSuccess: response => Results.Ok(response),
                    onError: error => Results.Extensions.FromError(error));
            })
            .WithName(nameof(GetPortfolio));

        return endpoints;
    }

    public static IServiceCollection AddGetPortfolioServices(this IServiceCollection services)
    {
        services.AddScoped<GetPortfolioHandler>();
        services.AddScoped<GetPortfolioRequestValidator>();

        return services;
    }

    public class GetPortfolioHandler(ReadingDbContext dbContext, GetPortfolioRequestValidator validator)
    {
        public async Task<Result<GetPortfolioResponse>> Handle(GetPortfolioRequest request)
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return NewError(validationResult);
            }

            var portfolio = await dbContext.Portfolios
                .Where(p => p.Id == request.Id)
                .Select(p => new GetPortfolioResponse(p.Id, p.Name))
                .SingleOrDefaultAsync();

            if (portfolio is null)
            {
                return NewItemNotFoundError($"Portfolio {request.Id} not found.");
            }

            return portfolio;
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
