using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace ApiJWT.Essentials;

public sealed class UserModel
{
    [JsonIgnore]
    public string JwtId { get; set; } = Guid.NewGuid().ToString();
    public int Id { get; private set; } = 0;
    public string Name { get; private set; } = string.Empty;
    public IEnumerable<string> Roles { get; private set; } = [];

    public UserModel(int id, string name, IEnumerable<string> roles)
    {
        Id = id;
        Name = name;
        Roles = roles;
    }

    public UserModel(IEnumerable<Claim> claims)
    {
        var roles = new List<string>();

        foreach (Claim claim in claims)
        {
            switch (claim.Type)
            {
                // During refresh token JwtSecurityToken.Claims is used and it has different claim name
                case ClaimTypes.NameIdentifier or JwtRegisteredClaimNames.NameId:
                    Id = int.Parse(claim.Value);
                    break;
                case ClaimTypes.Name or JwtRegisteredClaimNames.UniqueName:
                    Name = claim.Value;
                    break;
                case JwtRegisteredClaimNames.Jti:
                    JwtId = claim.Value;
                    break;
                case ClaimTypes.Role or "role":
                    roles.Add(claim.Value);
                    break;
            }
        }

        Roles = roles;
    }

    public IEnumerable<Claim> ToClaims()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, Id.ToString()), // JwtRegisteredClaimNames.Sub
            new Claim(ClaimTypes.Name, Name),
            new Claim(JwtRegisteredClaimNames.Jti, JwtId)
        };

        claims.AddRange(Roles.Select(role => new Claim(ClaimTypes.Role, role)));

        return claims;
    }
}
