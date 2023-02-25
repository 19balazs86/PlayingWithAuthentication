namespace BlazorWASM.BFF.Auth0.OpenIDConnect.Shared.DTO;

public readonly record struct ClaimValue(string Type, string Value);

public sealed class UserInfo
{
    public static readonly UserInfo Anonymous = new();

    public bool IsAuthenticated { get; set; }

    public string NameClaimType { get; set; }

    public string RoleClaimType { get; set; }

    public IEnumerable<ClaimValue> Claims { get; set; } = Enumerable.Empty<ClaimValue>();
}
