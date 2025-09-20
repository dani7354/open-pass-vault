using OpenPassVault.API.Data.Interfaces;
using OpenPassVault.API.Services.Interfaces;
using OpenPassVault.Shared.DTO;
using SecretEntity = OpenPassVault.API.Data.Entity.Secret;

namespace OpenPassVault.API.Services;

public class SecretService(ISecretRepository secretRepository) : ISecretService
{
    public async Task<SecretDetailsResponse?> GetAsync(string id, string userId)
    {
        var secretEntity = await secretRepository.GetSecret(id, userId);
        if (secretEntity == null) return null;
        
        return new SecretDetailsResponse(
            Id: secretEntity.Id,
            Name: secretEntity.Name,
            Username: secretEntity.Username ?? "",
            Type:  secretEntity.Type,
            Description: secretEntity.Description ?? "",
            Created: secretEntity.Created.ToString("o"),
            Updated: secretEntity.Updated.ToString("o"),
            Content: secretEntity.Content
        );
    }

    public async Task<IList<SecretListItemResponse>> ListAsync(string userId)
    {
        var secretsForUser = await secretRepository.ListForUser(userId);
        return secretsForUser.Select(
            x => new SecretListItemResponse
            {
                Id = x.Id,
                Name = x.Name,
                Username = x.Username ?? "",
                Type = x.Type,
            }).ToList();
    }

    public async Task<string> CreateAsync(SecretCreateRequest secret, string userId)
    {
        var secretEntity = new SecretEntity
        {
            Id = Guid.NewGuid().ToString(),
            Name = secret.Name,
            Description = secret.Description,
            Username = secret.Username,
            Type = secret.Type,
            Content = secret.Content,
            UserId = userId,
            Created = DateTime.Now,
            Updated = DateTime.Now
        };
        
        var secretId = await secretRepository.Add(secretEntity);
        return secretId;
    }

    public async Task UpdateAsync(SecretUpdateRequest secret, string userId)
    {
        var secretEntity = new SecretEntity
        {
            Id = secret.Id,
            Name = secret.Name,
            Description = secret.Description,
            Username = secret.Username,
            Type = secret.Type,
            Content = secret.Content,
            Updated = DateTime.Now
        };
        
        await secretRepository.Update(secretEntity, userId);
    }

    public Task DeleteAsync(string id, string userId)
    {
        return secretRepository.Delete(id);
    }
}