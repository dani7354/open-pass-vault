using OpenPassVault.Shared.DTO;

namespace OpenPassVault.API.Services.Interfaces;

public interface ISecretService
{
    Task<SecretDetailsResponse?> GetAsync(string id, string userId);
    Task<IList<SecretListItemResponse>> ListAsync(string userId);
    Task<string> CreateAsync(SecretCreateRequest secret, string userId);
    Task UpdateAsync(SecretUpdateRequest secret, string userId);
    Task DeleteAsync(string id, string userId);
    
}