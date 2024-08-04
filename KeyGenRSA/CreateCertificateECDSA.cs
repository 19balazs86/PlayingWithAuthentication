using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace KeyGenRSA;

public static class CreateCertificateECDSA
{
    public static async Task DoIt()
    {
        using ECDsa ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);

        string privateKeyPem = ecdsa.ExportECPrivateKeyPem();
        string publicKeyPem  = ecdsa.ExportSubjectPublicKeyInfoPem();

        var certificateRequest = new CertificateRequest("CN=My-SelfSigned-Cert", ecdsa, HashAlgorithmName.SHA256);

        using X509Certificate2 certificate = certificateRequest.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(1));

        string certificateText = exportAsCertText(certificate);

        await File.WriteAllTextAsync("Key-ECDsa-Private.key", privateKeyPem);
        await File.WriteAllTextAsync("Key-ECDsa-Public.key",  publicKeyPem);
        await File.WriteAllTextAsync("Key-ECDsa-Cert.crt",    certificateText);

        // X509Certificate2 certificateFromFile = X509Certificate2.CreateFromPemFile("Key-ECDsa-Cert.crt", "Key-ECDsa-Private.key");
    }

    private static string exportAsCertText(X509Certificate2 certificate)
    {
        byte[] certificateBytes = certificate.Export(X509ContentType.Cert, "password");

        var stringBuilder = new StringBuilder();

        stringBuilder.AppendLine("-----BEGIN CERTIFICATE-----");
        stringBuilder.AppendLine(Convert.ToBase64String(certificateBytes, Base64FormattingOptions.InsertLineBreaks));
        stringBuilder.AppendLine("-----END CERTIFICATE-----");

        return stringBuilder.ToString();
    }
}
