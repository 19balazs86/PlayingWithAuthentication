using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ApiKeyAuth.Solutions;

public sealed class ApiKeyAuthAttribute : ServiceFilterAttribute
{
    public ApiKeyAuthAttribute() : base(typeof(ApiKeyAuthFilter))
    {
    }
}

public sealed class ApiKeyAuthFilter : IAuthorizationFilter // IAsyncAuthorizationFilter
{
    private readonly IApiKeyValidator _apiKeyValidator;

    public ApiKeyAuthFilter(IApiKeyValidator apiKeyValidator)
    {
        _apiKeyValidator = apiKeyValidator;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        bool isValid = _apiKeyValidator.Validate(context.HttpContext.Request);

        if (!isValid)
        {
            context.Result = new UnauthorizedObjectResult(InvalidApiKeyProblemDetails.Instance);
        }
    }
}

public sealed class InvalidApiKeyProblemDetails
{
    public static readonly InvalidApiKeyProblemDetails Instance = new();

    public int Status { get; }   = StatusCodes.Status401Unauthorized;

    public string Title { get; } = "Invalid API Key";
}
