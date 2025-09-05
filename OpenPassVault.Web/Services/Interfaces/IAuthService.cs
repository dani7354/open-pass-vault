using OpenPassVault.Shared.DTO;

namespace OpenPassVault.Web.Services.Interfaces;

public interface IAuthService
{
    Task<string?> GetPersistedTokenAsync();
    Task<bool> RegisterAsync(RegisterRequest accountDto);
    Task<bool> LoginAsync(LoginRequest loginDto);
    Task LogoutAsync();
}
