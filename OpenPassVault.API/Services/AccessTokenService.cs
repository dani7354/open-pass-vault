using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using OpenPassVault.API.Data.Entity;
using OpenPassVault.API.Services.Interfaces;
using OpenPassVault.Shared.Auth;

namespace OpenPassVault.API.Services;

public class AccessTokenService(byte[] tokenSigningKey, string tokenAudience, string tokenIssuer)
    : IAccessTokenService
{
    public int TokenExpirationDays => 1;

    public string CreateToken(ApiUser user, string sessionId, IEnumerable<Claim> userClaims)
    {
        var userId = user.Id;
        var userName = user.UserName!;
        var email = user.Email!;
        var masterPasswordHash = user.MasterPasswordHash;

        var claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.NameId, userId),
            new (JwtRegisteredClaimNames.Name, userName),
            new (JwtRegisteredClaimNames.Email, email),
            new (JwtRegisteredClaimNames.UniqueName, userName),
            new (JwtClaimType.TokenMasterPasswordHashClaimType, masterPasswordHash),
            new (JwtClaimType.SessionId, sessionId)
        };

        claims.AddRange(userClaims);

        var key = new SymmetricSecurityKey(tokenSigningKey);
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = tokenIssuer,
            Audience = tokenAudience,
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(TokenExpirationDays),
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}