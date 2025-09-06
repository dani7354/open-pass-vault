using System.Text;
using OpenPassVault.Shared.Crypto.Interfaces;
using System.Security.Cryptography;

namespace OpenPassVault.Shared.Crypto;

public class EncryptionService(ISymmetricKeyGenerator keyGenerator) : IEncryptionService
{
    private const int KeySize = 32;
    
    public async Task<string> Encrypt(string plainText, string password)
    {
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        var keyResponse = await keyGenerator.GenerateKey(password, KeySize);
        
        var randomGenerator = RandomNumberGenerator.Create();
        var nonce = new byte[AesGcm.NonceByteSizes.MaxSize];
        randomGenerator.GetBytes(nonce);
        
        var cipherText = new byte[plainTextBytes.Length];
        var tag =  new byte[AesGcm.TagByteSizes.MaxSize];

        var aes = new AesGcm(keyResponse.Key, AesGcm.TagByteSizes.MaxSize);
        aes.Encrypt(nonce, plainTextBytes, cipherText, tag);

        return CipherTextToBase64(cipherText, nonce, tag, keyResponse.Salt);
    }
    
    // Cipher text format:  | nonce | cipherText | tag | keySalt |
    public async Task<string> Decrypt(string cipherText, string password)
    {
        var fullCipherTextBytes = Convert.FromBase64String(cipherText);
        var nonce = fullCipherTextBytes.Take(AesGcm.NonceByteSizes.MaxSize).ToArray();

        var cipherTextLength = fullCipherTextBytes.Length - nonce.Length - AesGcm.TagByteSizes.MaxSize - keyGenerator.SaltLength;
        var cipherTextBytes = fullCipherTextBytes
            .Skip(AesGcm.NonceByteSizes.MaxSize)
            .Take(cipherTextLength)
            .ToArray();
        
        var tagBytes = fullCipherTextBytes
            .Skip(fullCipherTextBytes.Length - AesGcm.TagByteSizes.MaxSize)
            .Take(AesGcm.TagByteSizes.MaxSize)
            .ToArray();

        var keySalt = fullCipherTextBytes
            .Skip(fullCipherTextBytes.Length - keyGenerator.SaltLength)
            .ToArray();
        
        var keyResponse = await keyGenerator.GenerateKey(password, keySalt, KeySize);
        var aes = new AesGcm(keyResponse.Key, AesGcm.TagByteSizes.MaxSize);
        var plainTextBytes = new byte[cipherTextLength];
        aes.Decrypt(nonce, cipherTextBytes, tagBytes, plainTextBytes);
        
        return Encoding.UTF8.GetString(plainTextBytes);
    }

    private string CipherTextToBase64(byte[] cipherText, byte[] nonce, byte[] tag, byte[] keySalt)
    {
        var fullCipherText = nonce
            .Concat(cipherText)
            .Concat(tag)
            .Concat(keySalt)
            .ToArray();
        
        return Convert.ToBase64String(fullCipherText);
    }
}