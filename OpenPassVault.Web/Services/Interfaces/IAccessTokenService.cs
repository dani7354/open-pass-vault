namespace OpenPassVault.Web.Services.Interfaces;

public interface IAccessTokenService
{
    string? GetToken();
    void SaveToken(string token);
    void RemoveToken();
}