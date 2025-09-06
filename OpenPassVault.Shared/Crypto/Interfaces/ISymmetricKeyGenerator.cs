namespace OpenPassVault.Shared.Crypto.Interfaces;

public interface ISymmetricKeyGenerator
{
    int SaltLength { get; }
    Task<KeyResponse> GenerateKey(string password, int length);
    Task<KeyResponse> GenerateKey(string password, byte[] salt, int length);
}