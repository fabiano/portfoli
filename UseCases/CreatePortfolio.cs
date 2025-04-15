namespace Portfoli.UseCases;

public static class CreatePortfolio
{
    public static RouteGroupBuilder MapCreatePortfolioEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/", async (CreatePortfolioRequest request, CreatePortfolioHandler handler) =>
        {
            var result = await handler.Handle(request);

            return result.Match(
                onSuccess: response => Results.Created($"/portfolios/{response.Id}", response),
                onError: error => Results.Extensions.FromError(error));
        });

        return group;
    }

    public static IServiceCollection AddCreatePortfolioServices(this IServiceCollection services)
    {
        services.AddScoped<CreatePortfolioHandler>();
        services.AddScoped<CreatePortfolioRequestValidator>();

        return services;
    }

    public class CreatePortfolioHandler(IUnitOfWork unitOfWork, CreatePortfolioRequestValidator validator)
    {
        public async Task<Result<CreatePortfolioResponse>> Handle(CreatePortfolioRequest request)
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return Error.New(validationResult);
            }

            var portfolio = new Portfolio { Name = request.Name };

            await unitOfWork.Portfolios.Add(portfolio);
            await unitOfWork.SaveChanges();

            return new CreatePortfolioResponse(portfolio.Id);
        }
    }

    public record CreatePortfolioRequest(string Name);

    public record CreatePortfolioResponse(PortfolioId Id);

    public class CreatePortfolioRequestValidator : AbstractValidator<CreatePortfolioRequest>
    {
        public CreatePortfolioRequestValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty()
                .MaximumLength(512);
        }
    }
}
