namespace Portfoli.UseCases;

public static class CreateHolding
{
    public static IEndpointRouteBuilder MapCreateHoldingEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/portfolios/{portfolioId:guid}/holdings", async (
            [FromRoute] PortfolioId portfolioId,
            [FromBody] CreateHoldingRequest request,
            [FromServices] CreateHoldingRequestValidator validator,
            [FromServices] IUnitOfWork unitOfWork) =>
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var portfolio = await unitOfWork.Portfolios.Get(portfolioId);

            if (portfolio is null)
            {
                return Results.NotFound($"Portfolio {portfolioId} not found.");
            }

            var asset = await unitOfWork.Assets.GetByTicker(request.Exchange, request.Ticker);

            if (asset is null)
            {
                return Results.Problem($"Asset {request.Ticker} on {request.Exchange} not found.");
            }

            var holding = new Holding
            {
                Asset = asset,
            };

            portfolio.AddHolding(holding);

            await unitOfWork.SaveChanges();

            var response = new CreateHoldingResponse(holding.Id);

            return Results.Created($"/portfolios/{portfolioId}/holdings/{response.Id}", response);
        });

        return endpoints;
    }

    public static IServiceCollection AddCreateHoldingServices(this IServiceCollection services) =>
        services.AddScoped<CreateHoldingRequestValidator>();

    public record CreateHoldingRequest(string Exchange, string Ticker);

    public record CreateHoldingResponse(HoldingId Id);

    public class CreateHoldingRequestValidator : AbstractValidator<CreateHoldingRequest>
    {
        public CreateHoldingRequestValidator()
        {
            RuleFor(p => p.Exchange)
                .NotEmpty();

            RuleFor(p => p.Ticker)
                .NotEmpty();
        }
    }
}
