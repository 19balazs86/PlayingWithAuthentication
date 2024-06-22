using System.Security.Cryptography;

namespace KeyGenRSA;

public static class Hashing_PBKDF2
{
    private const int _keySize    = 64;
    private const int _iterations = 350000;

    private static readonly HashAlgorithmName _hashAlgorithmName = HashAlgorithmName.SHA512;

    public static string HashPasword(ReadOnlySpan<char> password, out ReadOnlySpan<byte> salt)
    {
        salt = RandomNumberGenerator.GetBytes(_keySize);

        byte[] passwordHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, _iterations, _hashAlgorithmName, _keySize);

        return Convert.ToHexString(passwordHash);
    }

    public static bool VerifyPassword(ReadOnlySpan<char> password, ReadOnlySpan<char> passwordHash, ReadOnlySpan<char> salt)
    {
        byte[] saltBytes = Convert.FromHexString(salt);

        return VerifyPassword(password, passwordHash, saltBytes);
    }

    public static bool VerifyPassword(ReadOnlySpan<char> password, ReadOnlySpan<char> passwordHash, ReadOnlySpan<byte> salt)
    {
        byte[] hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, _iterations, _hashAlgorithmName, _keySize);

        byte[] hashBytes = Convert.FromHexString(passwordHash);

        return hashToCompare.SequenceEqual(hashBytes);
    }

    public static bool TestHashing(ReadOnlySpan<char> password)
    {
        string passwordHash = HashPasword(password, out ReadOnlySpan<byte> saltBytes);

        //string saltYouCanStore = Convert.ToHexString(saltBytes);

        return VerifyPassword(password, passwordHash, saltBytes);
    }
}
