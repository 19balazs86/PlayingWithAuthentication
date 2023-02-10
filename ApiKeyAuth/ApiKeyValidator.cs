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
        return headers is not null && Validate(headers[ApiKeyHeaderName]);
    }

    public bool Validate(IQueryCollection query)
    {
        return query is not null && Validate(query[ApiKeyQueryName]);
    }

    public bool Validate(HttpRequest request)
    {
        return request is not null && (Validate(request.Headers) || Validate(request.Query));
    }
}
