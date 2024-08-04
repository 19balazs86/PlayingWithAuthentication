using System.Security.Cryptography;

namespace KeyGenRSA;

public static class Program
{
    public static async Task Main(string[] args)
    {
        using RSA keyRSA = RSA.Create();

        //byte[] privateKeyBytes = keyRSA.ExportRSAPrivateKey();
        //await File.WriteAllBytesAsync("KeyPrivateRSA", privateKeyBytes);
        //keyRSA.ImportRSAPrivateKey(privateKeyBytes, out _);

        string privateKeyPem = keyRSA.ExportRSAPrivateKeyPem();
        string publicKeyPem  = keyRSA.ExportRSAPublicKeyPem();

        await File.WriteAllTextAsync("Key-RSA-Private.pem", privateKeyPem);
        await File.WriteAllTextAsync("Key-RSA-Public.pem",  publicKeyPem);

        // await CreateCertificateECDSA.DoIt();
    }
}