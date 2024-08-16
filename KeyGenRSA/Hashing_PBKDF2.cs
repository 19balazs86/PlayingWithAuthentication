using System.Security.Cryptography;

namespace KeyGenRSA;

public static class Hashing_PBKDF2
{
    private const int _saltSize   = 32;
    private const int _hashSize   = 64;
    private const int _iterations = 350_000;

    private static readonly HashAlgorithmName _hashAlgorithmName = HashAlgorithmName.SHA512;

    public static string HashPasword(ReadOnlySpan<char> password, out ReadOnlySpan<byte> salt)
    {
        salt = RandomNumberGenerator.GetBytes(_saltSize);

        byte[] passwordHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, _iterations, _hashAlgorithmName, _hashSize);

        // Return value could be: Hash-Salt, which you can store in 1 field
        // return $"{Convert.ToHexString(passwordHash)}-{Convert.ToHexString(salt)}";

        return Convert.ToHexString(passwordHash);
    }

    public static bool VerifyPassword(ReadOnlySpan<char> password, ReadOnlySpan<char> passwordHash, ReadOnlySpan<char> salt)
    {
        byte[] saltBytes = Convert.FromHexString(salt);

        return VerifyPassword(password, passwordHash, saltBytes);
    }

    public static bool VerifyPassword(ReadOnlySpan<char> password, ReadOnlySpan<char> passwordHash, ReadOnlySpan<byte> salt)
    {
        byte[] hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, _iterations, _hashAlgorithmName, _hashSize);

        byte[] hashBytes = Convert.FromHexString(passwordHash);

        // return hashToCompare.SequenceEqual(hashBytes);

        return CryptographicOperations.FixedTimeEquals(hashToCompare, hashBytes); // More secure
    }

    public static bool TestHashing(ReadOnlySpan<char> password)
    {
        string passwordHash = HashPasword(password, out ReadOnlySpan<byte> saltBytes);

        //string saltYouCanStore = Convert.ToHexString(saltBytes);

        return VerifyPassword(password, passwordHash, saltBytes);
    }
}
