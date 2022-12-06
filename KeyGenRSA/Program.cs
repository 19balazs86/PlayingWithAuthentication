using System.Security.Cryptography;

using RSA keyRSA = RSA.Create();

//byte[] privateKeyBytes = keyRSA.ExportRSAPrivateKey();
//await File.WriteAllBytesAsync("KeyPrivateRSA", privateKeyBytes);
//keyRSA.ImportRSAPrivateKey(privateKeyBytes, out _);

string privateKeyPem = keyRSA.ExportRSAPrivateKeyPem();
string publicKeyPem  = keyRSA.ExportRSAPublicKeyPem();

await File.WriteAllTextAsync("Key-RSA-Private.pem", privateKeyPem);
await File.WriteAllTextAsync("Key-RSA-Public.pem",  publicKeyPem);