using System.Security.Cryptography;
using OpenPassVault.Shared.Crypto.Interfaces;

namespace OpenPassVault.Shared.Crypto;

public class PasswordHasher(ISymmetricKeyGenerator keyGenerator) : IPasswordHasher
{
    private const int DigestLength = 24;

    public async Task<string> HashPassword(string password)
    {
        var keyResponse = await keyGenerator.GenerateKey(password, DigestLength);

        return keyResponse.FullHexDigest;
    }

    public async Task<bool> VerifyPassword(string hashedPassword, string password)
    {
        var hashSplit = hashedPassword.Split(KeyResponse.Delimiter);
        if (hashSplit.Length != 2)
            return false;

        var saltBytes = Convert.FromHexString(hashSplit[1]);
        var digest = Convert.FromHexString(hashSplit[0]);
        var keyResponse = await keyGenerator.GenerateKey(password, saltBytes, DigestLength);

        return CryptographicOperations.FixedTimeEquals(keyResponse.Key, digest);
    }
}
