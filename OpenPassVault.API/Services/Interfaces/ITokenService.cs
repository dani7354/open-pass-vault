using System.Security.Claims;
using OpenPassVault.API.Data.Entity;

namespace OpenPassVault.API.Services.Interfaces;

public interface ITokenService
{
    string CreateToken(ApiUser user, IEnumerable<Claim> userClaims);
}