using System.Text;
using OpenPassVault.Shared.Crypto.Interfaces;
using System.Security.Cryptography;
using Konscious.Security.Cryptography;

namespace OpenPassVault.Shared.Crypto;

public class KeyGenerator : ISymmetricKeyGenerator
{
    // See configuration recommendations https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html
    private const int KeyLength = 32;
    private const int Iterations = 2;
    private const int MemSize = 19456;
    private const int DegreeOfParallelism = 1;

    public int SaltLength => 16;
    
    public async Task<KeyResponse> GenerateKey(string password, int length = KeyLength)
    {
        var randomGenerator = RandomNumberGenerator.Create();
        var salt = new byte[SaltLength];
        randomGenerator.GetBytes(salt);

        return await GenerateKey(password, salt, length);
    }

    public async Task<KeyResponse> GenerateKey(string password, byte[] salt, int length = KeyLength)
    {
        var passwordBytes = Encoding.UTF8.GetBytes(password);

        var argon = new Argon2id(passwordBytes)
        {
            Salt = salt,
            Iterations = Iterations,
            DegreeOfParallelism = DegreeOfParallelism,
            MemorySize = MemSize
        };

        var keyBytes = await argon.GetBytesAsync(KeyLength);

        return new KeyResponse(Key: keyBytes, Salt: salt);
    }
}