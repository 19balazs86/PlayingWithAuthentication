using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using System.Security.Claims;
using System.Text.Json;

namespace BlazorWASM.Auth0.JWT.OpenIDConnect.Client;

// Without this class, the role claim will be an array that cannot be used for role authorization.
// Claim: ../identity/claims/role = ["TestRole","TestRole2"]
public sealed class ArrayClaimsPrincipalFactory<TAccount> : AccountClaimsPrincipalFactory<TAccount> where TAccount : RemoteUserAccount
{
    public ArrayClaimsPrincipalFactory(IAccessTokenProviderAccessor accessor) : base(accessor)
    {
    }

    public override async ValueTask<ClaimsPrincipal> CreateUserAsync(TAccount account, RemoteAuthenticationUserOptions options)
    {
        ClaimsPrincipal principal = await base.CreateUserAsync(account, options);

        ClaimsIdentity? identity = principal.Identity as ClaimsIdentity;

        if (account is null || identity is null) return principal;

        foreach (var (key, value) in account.AdditionalProperties)
        {
            // We are only interested in JSON arrays
            if (value is not JsonElement { ValueKind: JsonValueKind.Array } element) continue;

            identity.RemoveClaim(identity.FindFirst(key));

            Claim[] claims = element
                .EnumerateArray()
                .Select(x => new Claim(key, x.ToString()))
                .ToArray();

            identity.AddClaims(claims);
        }

        return principal;
    }
}