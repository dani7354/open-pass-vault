using System.Security.Claims;
using OpenPassVault.Shared.DTO;
using OpenPassVault.Web.Models;

namespace OpenPassVault.Web.Services.Interfaces;

public interface IAuthService
{
    Task<ClaimsPrincipal?> GetClaimsPrincipalFromToken();
    Task<bool> RegisterAsync(RegisterViewModel accountDto);
    Task<bool> LoginAsync(LoginRequest loginDto);
    Task LogoutAsync();
}
