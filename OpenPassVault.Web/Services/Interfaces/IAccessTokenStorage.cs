namespace OpenPassVault.Web.Services.Interfaces;

public interface IAccessTokenStorage
{
    Task<string?> GetToken();
    Task SetToken(string token);
    Task ClearToken();
}