namespace OpenPassVault.API.Services.Interfaces;

public interface ICsrfTokenService
{
    Task<string> GenerateToken(string uniqueId);
    Task<bool> ValidateToken(string uniqueId, string fullToken);
}