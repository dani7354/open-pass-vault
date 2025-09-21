using OpenPassVault.Shared.Crypto.Interfaces;
using Blazor.SubtleCrypto;
using Microsoft.JSInterop;

namespace OpenPassVault.Web.Services;

public class SubtleCryptoWrapper(ISymmetricKeyGenerator keyGenerator, IJSRuntime jsRuntime) : IEncryptionService
{
    private const int KeySize = 32;
    
    public async Task<string> Encrypt(string plainText, string password)
    {
        var keyResponse = await keyGenerator.GenerateKey(password, KeySize);
        var cryptoService = new CryptoService(jsRuntime, new CryptoOptions
        {
            Key = keyResponse.KeyBase64
        });
        
        var result = await cryptoService.EncryptAsync(plainText);
        var fullCipherText = Convert.FromBase64String(result.Value)
            .Concat(keyResponse.Salt)
            .ToArray();
        
        return Convert.ToBase64String(fullCipherText);
    }

    public async Task<string> Decrypt(string cipherText, string password)
    {
        var fullCipherTextBytes = Convert.FromBase64String(cipherText);
        var cipherTextLength = fullCipherTextBytes.Length - keyGenerator.SaltLength;
        var cipherTextBytes = fullCipherTextBytes.Take(cipherTextLength).ToArray();
        
        var salt = fullCipherTextBytes.Skip(cipherTextLength).ToArray();
        var keyResponse = await keyGenerator.GenerateKey(password, salt, KeySize);
        var service = new CryptoService(jsRuntime, new CryptoOptions
        {
            Key = keyResponse.KeyBase64
        });

        return await service.DecryptAsync(Convert.ToBase64String(cipherTextBytes));
    }
}