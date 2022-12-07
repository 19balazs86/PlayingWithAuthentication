using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace ApiJWT
{
    public sealed class UserModel
    {
        [JsonIgnore]
        public string JwtId { get; set; } = Guid.NewGuid().ToString();
        public int Id { get; private set; } = 0;
        public string Name { get; private set; } = string.Empty;
        public IEnumerable<string> Roles { get; private set; } = Enumerable.Empty<string>();

        public UserModel(int id, string name, IEnumerable<string> roles)
        {
            Id    = id;
            Name  = name;
            Roles = roles;
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
                    case JwtRegisteredClaimNames.Jti:
                        JwtId = claim.Value;
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
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, Id.ToString()), // JwtRegisteredClaimNames.Sub
                new Claim(ClaimTypes.Name, Name),
                new Claim(JwtRegisteredClaimNames.Jti, JwtId),

                new Claim(ClaimTypes.Role, "DefaultRole")
            };

            claims.AddRange(Roles.Select(r => new Claim(ClaimTypes.Role, r)));

            return claims;
        }
    }
}
