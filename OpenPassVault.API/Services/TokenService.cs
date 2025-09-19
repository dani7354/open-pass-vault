using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using OpenPassVault.API.Data.Entity;
using OpenPassVault.API.Services.Interfaces;
using OpenPassVault.Shared.Auth;

namespace OpenPassVault.API.Services;

public class TokenService : ITokenService
{
    private const int TokenExpirationDays = 1;

    private readonly byte[] _tokenSigningKey;
    private readonly string _tokenIssuer;
    private readonly string _tokenAudience;

    public TokenService(byte[] tokenSigningKey, string tokenAudience, string tokenIssuer)
    {
        _tokenSigningKey = tokenSigningKey;
        _tokenIssuer = tokenIssuer;
        _tokenAudience = tokenAudience;
    }
    
    public string CreateToken(ApiUser user, IEnumerable<Claim> userClaims)
    {
        var userId = user.Id;
        var userName = user.UserName!;
        var email = user.Email!;
        var masterPasswordHash = user.MasterPasswordHash;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.NameId, userId),
            new(JwtRegisteredClaimNames.UniqueName, userName),
            new (JwtRegisteredClaimNames.Email, email),
            new (JwtClaimType.TokenMasterPasswordHashClaimType, masterPasswordHash)
        };

        claims.AddRange(userClaims);

        var key = new SymmetricSecurityKey(_tokenSigningKey);
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _tokenIssuer,
            Audience = _tokenAudience,
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(TokenExpirationDays),
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
    
}