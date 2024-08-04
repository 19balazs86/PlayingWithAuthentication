using System.Security.Claims;

namespace ApiCookieAuth.Essentials;

public sealed class UserModel
{
    public const string SessionClaimName = nameof(SessionId);

    public string SessionId { get; private set; } = Guid.NewGuid().ToString();
    public int Id { get; private set; } = 0;
    public string Name { get; private set; } = string.Empty;
    public IEnumerable<string> Roles { get; private set; } = [];

    public UserModel(int id, string name) : this(id, name, [])
    {
    }

    public UserModel(int id, string name, IEnumerable<string> roles)
    {
        Id    = id;
        Name  = name;
        Roles = roles;
    }

    public static UserModel CreateFromPrincipal(ClaimsPrincipal principal)
    {
        if (!principal.Claims.Any())
        {
            return new UserModel(0, "You are not logged in");
        }

        return new UserModel(principal.Claims);
    }

    public UserModel(IEnumerable<Claim> claims)
    {
        var roles = new List<string>();

        foreach (Claim claim in claims)
        {
            switch (claim.Type)
            {
                case ClaimTypes.NameIdentifier:
                    Id = int.Parse(claim.Value);
                    break;
                case ClaimTypes.Name:
                    Name = claim.Value;
                    break;
                case SessionClaimName:
                    SessionId = claim.Value;
                    break;
                case ClaimTypes.Role:
                    roles.Add(claim.Value);
                    break;
            }
        }

        Roles = roles;
    }

    public IEnumerable<Claim> ToClaims()
    {
        List<Claim> claims =
        [
            new Claim(ClaimTypes.NameIdentifier, Id.ToString()),
            new Claim(ClaimTypes.Name,           Name),
            new Claim(SessionClaimName,          SessionId)
        ];

        claims.AddRange(Roles.Select(r => new Claim(ClaimTypes.Role, r)));

        return claims;
    }
}
