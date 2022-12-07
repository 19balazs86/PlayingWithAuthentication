namespace ApiJWT
{
    // For simplicity
    public static class RefreshTokenRepository
    {
        public static Dictionary<string, RefreshToken> RefreshToken = new Dictionary<string, RefreshToken>();

        public static string CreateRefreshToken(string jwtId)
        {
            var refreshToken = new RefreshToken
            {
                Token      = generateRefreshToken(),
                JwtId      = jwtId,
                DateCreate = DateTime.UtcNow,
                DateExpiry = DateTime.UtcNow.AddMonths(1)
            };

            RefreshToken.Add(jwtId, refreshToken);

            return refreshToken.Token;
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
        public required DateTime DateCreate { get; set; }
        public required DateTime DateExpiry { get; set; }
        public bool IsUsed { get; set; }
        public bool IsValid { get; set; } = true;
    }
}
