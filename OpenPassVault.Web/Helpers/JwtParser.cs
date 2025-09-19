using System.Diagnostics;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace OpenPassVault.Web.Helpers;

public static class JwtParser
{
    private const string AuthenticationType = "OpenPassVault.API";
    
    public static ClaimsPrincipal ToClaimsPrincipal(string jwt)
    {
        var token = new JwtSecurityTokenHandler().ReadJwtToken(jwt);
        Debug.WriteLine(token);
        Console.WriteLine(token);
        return new ClaimsPrincipal(new ClaimsIdentity(token.Claims, AuthenticationType));
    }
}