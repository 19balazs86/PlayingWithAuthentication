namespace ApiJWT.Essentials;

// For simplicity
public static class RefreshTokenRepository
{
    // Obviously you use a Database for this purpose
    private static readonly Dictionary<string, TokenData> _tokenStorage = [];

    public static string CreateRefreshToken(string jwtId)
    {
        var refreshToken = new TokenData
        {
            RefreshToken = generateRefreshToken(),
            JwtId = jwtId,
            CreationDate = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddMonths(1)
        };

        _tokenStorage.Add(jwtId, refreshToken);

        return refreshToken.RefreshToken;
    }

    public static bool TryInvalidateRefreshToken(string jwtId, string refreshToken)
    {
        if (_tokenStorage.TryGetValue(jwtId, out TokenData? tokenData) &&
            tokenData is { IsValid: true, IsUsed: false } &&
            tokenData.RefreshToken.Equals(refreshToken) &&
            tokenData.ExpiryDate > DateTime.UtcNow)
        {
            tokenData.IsValid = false;
            tokenData.IsUsed = true;

            return true;
        }

        return false;
    }

    public static bool IsValidToken(string jwtId)
    {
        return _tokenStorage.TryGetValue(jwtId, out TokenData? tokenData) && tokenData.IsValid && tokenData.ExpiryDate > DateTime.UtcNow;
    }

    public static void InvalidateToken(string jwtId)
    {
        if (_tokenStorage.TryGetValue(jwtId, out TokenData? tokenData))
        {
            tokenData.IsValid = false;
        }
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

public sealed class TokenData
{
    public required string RefreshToken { get; set; }
    public required string JwtId { get; set; }
    public required DateTime CreationDate { get; set; }
    public required DateTime ExpiryDate { get; set; }
    public bool IsUsed { get; set; }
    public bool IsValid { get; set; } = true;
}
