using System.Security.Claims;

namespace BlazorWASM.Auth0.JWT.OpenIDConnect.Shared;

public readonly record struct ClaimValue(string Type, string Value)
{
    public static ClaimValue FromClaim(Claim claim) => new ClaimValue(claim.Type, claim.Value);

    public Claim ToClaim() => new Claim(Type, Value);
}

public sealed class UserInfo
{
    public static readonly UserInfo Anonymous = new();

    public bool IsAuthenticated { get; init; }

    public string NameClaimType { get; init; } = string.Empty;

    public string RoleClaimType { get; init; } = string.Empty;

    public IEnumerable<ClaimValue> Claims { get; init; } = Enumerable.Empty<ClaimValue>();

    public static UserInfo FromClaimsPrincipal(ClaimsPrincipal principal)
    {
        ClaimsIdentity? identity = principal.Identity as ClaimsIdentity;

        if (identity is { IsAuthenticated: true })
        {
            ClaimValue[] claims = identity.Claims.Select(ClaimValue.FromClaim).ToArray();

            return new UserInfo
            {
                IsAuthenticated = true,
                NameClaimType = identity.NameClaimType,
                RoleClaimType = identity.RoleClaimType,
                Claims = claims
            };
        }

        return Anonymous;
    }
}

public static class UserInfoExtentsion
{
    public static ClaimsPrincipal ToClaimsPrincipal(this UserInfo? user, string authenticationType)
    {
        if (user is null || !user.IsAuthenticated)
        {
            return new ClaimsPrincipal(new ClaimsIdentity());
        }

        Claim[] claims = user.Claims.Select(cv => cv.ToClaim()).ToArray();

        var identity = new ClaimsIdentity(claims, authenticationType, user.NameClaimType, user.RoleClaimType);

        return new ClaimsPrincipal(identity);
    }
}