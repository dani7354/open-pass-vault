using OpenPassVault.API.Services.Interfaces;
using System.Security.Cryptography;
using OpenPassVault.Shared.Crypto.Interfaces;

namespace OpenPassVault.API.Services;

public class CsrfTokenService(IHmacService hmacService) : ICsrfTokenService
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
        var tokenSplit = fullToken.Split(Delimiter);
        if (tokenSplit.Length != 2)
            return false;
        
        var tokenHmac = tokenSplit[0];
        var tokenRandom = Convert.FromHexString(tokenSplit[1]);

        var inputBytes = new Guid(uniqueId).ToByteArray().Concat(tokenRandom).ToArray();

        return await hmacService.VerifyHmac(tokenHmac, inputBytes);
    }

    private async Task<string> ComputeHmac(byte[] uniqueId, byte[] tokenRandom)
    {
        var inputBytes = uniqueId.Concat(tokenRandom).ToArray();
        var hmac = await hmacService.CreateHmac(inputBytes);

        return hmac;
    }
}