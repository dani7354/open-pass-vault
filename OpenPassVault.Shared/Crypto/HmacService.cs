using System.Security.Cryptography;
using OpenPassVault.Shared.Crypto.Interfaces;

namespace OpenPassVault.Shared.Crypto;

public class HmacService(byte[] key) : IHmacService
{
    public async Task<bool> VerifyHmac(string hmacDigest, byte[] data)
    {
        var calculatedHmac = await ComputeHmac(data);
        var hmacBytes = Convert.FromHexString(hmacDigest);

        return CryptographicOperations.FixedTimeEquals(calculatedHmac, hmacBytes);
    }

    public async Task<string> CreateHmac(byte[] data)
    {
        var hash = await ComputeHmac(data);
        
        return Convert.ToHexString(hash);
    }

    private async Task<byte[]> ComputeHmac(byte[] data)
    {
        var hmac = new HMACSHA256(key);
        var hash = await hmac.ComputeHashAsync(new MemoryStream(data));
        
        return hash;
    }
}