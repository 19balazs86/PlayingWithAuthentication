using Microsoft.Extensions.Primitives;

namespace ApiKeyAuth;


public interface IApiKeyValidator
{
    public bool Validate(string apiKey);
    public bool Validate(IHeaderDictionary headers);
    public bool Validate(IQueryCollection query);
    public bool Validate(HttpRequest request);
}

public sealed class ApiKeyValidator : IApiKeyValidator
{
    public const string ApiKeyHeaderName = "X-Api-Key";
    public const string ApiKeyQueryName  = "api-key";

    // You can read it from Redis or Database...
    private readonly HashSet<string> _validaKeys = new(1, StringComparer.OrdinalIgnoreCase) { "3bf51fe82c6f48a9b53f813ff7a3b30c" };

    public bool Validate(string apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            return false;

        return _validaKeys.Contains(apiKey);
    }

    public bool Validate(IHeaderDictionary headers)
    {
        if (headers is null || !headers.TryGetValue(ApiKeyHeaderName, out StringValues apiKey))
            return false;

        return Validate(apiKey);
    }

    public bool Validate(IQueryCollection query)
    {
        if (query is null || !query.TryGetValue(ApiKeyQueryName, out StringValues apiKey))
            return false;

        return Validate(apiKey);
    }

    public bool Validate(HttpRequest request)
    {
        return request is not null && Validate(request.Headers) || Validate(request.Query);
    }
}
