using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace ApiJWT.Essentials;

public sealed class UserModel
{
    public const string NameClaimType = JwtRegisteredClaimNames.Name;
    public const string RoleClaimType = "role";

    [JsonIgnore]
    public string JwtId { get; set; } = Guid.NewGuid().ToString();
    public int Id { get; private set; } = 0;
    public string Name { get; private set; } = string.Empty;
    public IEnumerable<string> Roles { get; private set; } = [];

    public UserModel(int id, string name, IEnumerable<string> roles)
    {
        Id    = id;
        Name  = name;
        Roles = roles;
    }

    public UserModel(IEnumerable<Claim> claims)
    {
        List<string> roles = [];

        foreach (Claim claim in claims)
        {
            switch (claim.Type)
            {
                case JwtRegisteredClaimNames.NameId:
                    Id = int.Parse(claim.Value);
                    break;
                case JwtRegisteredClaimNames.Name:
                    Name = claim.Value;
                    break;
                case JwtRegisteredClaimNames.Jti:
                    JwtId = claim.Value;
                    break;
                case RoleClaimType:
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
            new Claim(JwtRegisteredClaimNames.NameId, Id.ToString()), // JwtRegisteredClaimNames.Sub
            new Claim(JwtRegisteredClaimNames.Name,   Name),
            new Claim(JwtRegisteredClaimNames.Jti,    JwtId)
        ];

        claims.AddRange(Roles.Select(role => new Claim(RoleClaimType, role)));

        return claims;
    }
}
