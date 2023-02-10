namespace ApiKeyAuth.Solutions;

public static class ApiKeyAuthMiddlewareExtensions
{
    public static IApplicationBuilder UseApiKeyAuthMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ApiKeyAuthMiddleware>();
    }
}

public sealed class ApiKeyAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IApiKeyValidator _apiKeyValidator;

    public ApiKeyAuthMiddleware(RequestDelegate next, IApiKeyValidator apiKeyValidator)
    {
        _next            = next;
        _apiKeyValidator = apiKeyValidator;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        bool isValid = _apiKeyValidator.Validate(httpContext.Request);

        if (!isValid)
        {
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;

            await httpContext.Response.WriteAsJsonAsync(InvalidApiKeyProblemDetails.Instance);

            return;
        }

        await _next(httpContext);
    }
}