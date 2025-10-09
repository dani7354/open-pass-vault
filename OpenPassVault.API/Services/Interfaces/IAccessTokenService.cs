using System.Security.Claims;
using OpenPassVault.API.Data.Entity;

namespace OpenPassVault.API.Services.Interfaces;

public interface IAccessTokenService
{
    int TokenExpirationDays { get; }
    string CreateToken(ApiUser user, string sessionId, IEnumerable<Claim> userClaims);
}