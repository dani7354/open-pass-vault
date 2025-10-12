namespace OpenPassVault.Shared.Crypto.Interfaces;

public interface IHmacService
{
    Task<bool> VerifyHmac(string hmacDigest, byte[] data);
    Task<string> CreateHmac(byte[] data);
}