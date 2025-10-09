using OpenPassVault.API.Services.Interfaces;
using System.Security.Cryptography;

namespace OpenPassVault.API.Services;

public class CsrfTokenService(byte[] key) : ICsrfTokenService
{
    private const int RandomValueBytesLength = 16;
    private const char Delimiter = '.';
    
    public async Task<string> GenerateToken(string uniqueId)
    {
        var uniqueIdBytes = new Guid(uniqueId).ToByteArray();
        var randomBytes = RandomNumberGenerator.GetBytes(RandomValueBytesLength);

        var hash = await ComputeHmac(uniqueIdBytes, randomBytes);

        return $"{hash}{Delimiter}{Convert.ToHexString(randomBytes)}";
    }

    public async Task<bool> ValidateToken(string uniqueId, string fullToken)
    {
        var uniqueIdBytes = new Guid(uniqueId).ToByteArray();
        var tokenSplit = fullToken.Split(Delimiter);
        var tokenHmac = tokenSplit[0];
        var tokenRandom = Convert.FromHexString(tokenSplit[1]);

        var calculatedTokenHmac =  await ComputeHmac(uniqueIdBytes, tokenRandom);
        
        return string.Equals(calculatedTokenHmac, tokenHmac, StringComparison.InvariantCultureIgnoreCase);
    }
    
    private async Task<string> ComputeHmac(byte[] uniqueId, byte[] tokenRandom)
    {
        var hmac = new HMACSHA256(key);
        var inputBytes = new Guid(uniqueId).ToByteArray().Concat(tokenRandom).ToArray();
        var hash = await hmac.ComputeHashAsync(new MemoryStream(inputBytes));
        
        return Convert.ToHexString(hash);
    }
}