namespace ApiKeyAuth.Solutions;


public sealed class ApiKeyAuthEndpointFilter : IEndpointFilter
{
    private readonly IApiKeyValidator _apiKeyValidator;

    public ApiKeyAuthEndpointFilter(IApiKeyValidator apiKeyValidator)
    {
        _apiKeyValidator = apiKeyValidator;
    }

    public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        bool isValid = _apiKeyValidator.Validate(context.HttpContext.Request);

        if (isValid)
        {
            return await next(context);
        }

        return new InvalidApiKeyResult();
    }
}

public sealed class InvalidApiKeyResult : IResult, IStatusCodeHttpResult
{
    public int? StatusCode => StatusCodes.Status401Unauthorized;

    public async Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.StatusCode = StatusCode.Value;

        await httpContext.Response.WriteAsJsonAsync(InvalidApiKeyProblemDetails.Instance);
    }
}