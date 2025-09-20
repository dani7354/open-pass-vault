using OpenPassVault.Shared.Crypto.Interfaces;
using Blazor.SubtleCrypto;
using Microsoft.JSInterop;

namespace OpenPassVault.Web.Services;

public class SubtleCryptoWrapper(
    ISymmetricKeyGenerator keyGenerator,
    IJSRuntime jsRuntime,
    ICryptoService cryptoService,
    ILogger<SubtleCryptoWrapper> logger) : IEncryptionService
{
    private const int KeySize = 32;
    
    public async Task<string> Encrypt(string plainText, string password)
    {
       
        var keyResponse = await keyGenerator.GenerateKey(password, KeySize);
        logger.LogInformation($"Key: {keyResponse.KeyBase64}");
        logger.LogInformation($"Salt: {keyResponse.SaltBase64}");

        var cryptoService = new CryptoService(jsRuntime, new CryptoOptions()
        {
            Key = keyResponse.KeyBase64
        });
        
        var result = await cryptoService.EncryptAsync(plainText);
        logger.LogInformation($"Key from output: {result.Secret.Key}, IV from output: {result.Secret.IV}");

        var fullCipherText = Convert.FromBase64String(result.Value)
            .Concat(keyResponse.Salt)
            .ToArray();
        
        return Convert.ToBase64String(fullCipherText);
    }

    public async Task<string> Decrypt(string cipherText, string password)
    {
        var fullCipherTextBytes = Convert.FromBase64String(cipherText);
        logger.LogInformation($"Full cipher text length: {fullCipherTextBytes.Length}");
        
        var cipherTextLength = fullCipherTextBytes.Length - keyGenerator.SaltLength;
        logger.LogInformation($"Cipher text length: {cipherTextLength}");
        var cipherTextBytes = fullCipherTextBytes.Take(cipherTextLength).ToArray();
        logger.LogInformation($"Cipher text bytes length: {cipherTextBytes.Length}");
        
        
        var salt = fullCipherTextBytes.Skip(cipherTextLength).ToArray();
        logger.LogInformation($"Salt length: {salt.Length}");
        
        var keyResponse = await keyGenerator.GenerateKey(password, salt, KeySize);
        logger.LogInformation($"Key: {keyResponse.KeyBase64}");
        logger.LogInformation($"Salt: {keyResponse.SaltBase64}");
        
        var cryptoInput = new CryptoInput
        {
            Value = Convert.ToBase64String(cipherTextBytes),
            Key = keyResponse.KeyBase64
        };
        
        var service = new CryptoService(jsRuntime, new CryptoOptions()
        {
            Key = keyResponse.KeyBase64
        });

        var decrypted = await service.DecryptAsync(cryptoInput.Value);
        logger.LogInformation($"decrypted length: {decrypted.Length}, value: {decrypted}");
        return decrypted;
    }
}