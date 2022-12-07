namespace ApiJWT
{
    // For simplicity
    public static class RefreshTokenRepository
    {
        private static readonly Dictionary<string, RefreshToken> _refreshToken = new Dictionary<string, RefreshToken>();

        public static string CreateRefreshToken(string jwtId)
        {
            var refreshToken = new RefreshToken
            {
                Token        = generateRefreshToken(),
                JwtId        = jwtId,
                CreationDate = DateTime.UtcNow,
                ExpiryDate   = DateTime.UtcNow.AddMonths(1)
            };

            _refreshToken.Add(jwtId, refreshToken);

            return refreshToken.Token;
        }

        public static bool TryInvalidateRefreshToken(string jwtId, string refreshToken)
        {
            if (_refreshToken.TryGetValue(jwtId, out RefreshToken rToken) &&
                rToken.Token.Equals(refreshToken) && rToken.IsValid && !rToken.IsUsed && rToken.ExpiryDate > DateTime.UtcNow)
            {
                rToken.IsValid = false;
                rToken.IsUsed  = true;

                return true;
            }

            return false;
        }

        private static string generateRefreshToken()
        {
            return Guid.NewGuid().ToString();

            //var randomNumber = new byte[64];
            //using var rng = RandomNumberGenerator.Create();
            //rng.GetBytes(randomNumber);
            //return Convert.ToBase64String(randomNumber);
        }
    }

    public sealed class RefreshToken
    {
        public required string Token { get; set; }
        public required string JwtId { get; set; }
        public required DateTime CreationDate { get; set; }
        public required DateTime ExpiryDate { get; set; }
        public bool IsUsed { get; set; }
        public bool IsValid { get; set; } = true;
    }
}
