namespace OpenPassVault.Shared.Crypto.Interfaces;

public interface IEncryptionService
{
    Task<string> Encrypt(string plainText, string password);
    Task<string> Decrypt(string cipherText, string password);
}