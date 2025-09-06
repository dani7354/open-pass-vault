using System.Security.Claims;
using OpenPassVault.Shared.DTO;

namespace OpenPassVault.Web.Services.Interfaces;

public interface IAuthService
{
    Task<ClaimsPrincipal?> GetClaimsPrincipalFromToken();
    Task<bool> RegisterAsync(RegisterRequest accountDto);
    Task<bool> LoginAsync(LoginRequest loginDto);
    Task LogoutAsync();
}
