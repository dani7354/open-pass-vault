using OpenPassVault.Shared.Crypto.Interfaces;
using OpenPassVault.Shared.DTO;
using OpenPassVault.Web.Models;
using OpenPassVault.Web.Providers;
using OpenPassVault.Web.Services.Exceptions;
using OpenPassVault.Web.Services.Interfaces;

namespace OpenPassVault.Web.Services;

public class SecretService(
    IHttpApiService httpApiService,
    ApiAuthenticationStateProvider authenticationStateProvider,
    IEncryptionService encryptionService,
    ILogger<SecretService> logger) : ISecretService
{
    private const string BaseUrl = "secrets";
    private const string masterPassword = "aaaaaaaaaaaaaaaaaaaaaaa"; 
    
    public async Task<IList<SecretListItemResponse>> ListSecrets()
    {
        try
        {
            var response = await httpApiService.GetAsync<IList<SecretListItemResponse>>(BaseUrl);
            return response ?? new List<SecretListItemResponse>();
        }
        catch (RequestUnauthorizedException)
        {
            await authenticationStateProvider.Logout();
            return new List<SecretListItemResponse>();
        }
    }

    public async Task<SecretDetailsResponse?> GetSecret(string id)
    {
        try
        {
            var response = await httpApiService.GetAsync<SecretDetailsResponse>($"{BaseUrl}/{id}");
            return response ?? null;
        }
        catch (RequestUnauthorizedException)
        {
            await authenticationStateProvider.Logout();
            return null;
        }
    }

    public async Task CreateSecret(SecretCreateViewModel secretCreateViewModel)
    {
        var encryptedContent =  await encryptionService.Encrypt(secretCreateViewModel.ContentPlain, masterPassword);
        var secretRequest = new SecretCreateRequest()
        {
            Name = secretCreateViewModel.Name,
            Description = secretCreateViewModel.Description,
            Username = secretCreateViewModel.Username,
            Type = secretCreateViewModel.Type,
            Content = encryptedContent
        };
        
        await httpApiService.PostAsync<SecretDetailsResponse>(BaseUrl, secretRequest);
    }
    
    public async Task DeleteSecret(string id)
    {
        try
        {
            await httpApiService.DeleteAsync($"{BaseUrl}/{id}");
        }
        catch (RequestUnauthorizedException)
        {
            await authenticationStateProvider.Logout();
        }
    }

    public async Task<string> DecryptSecretContent(string content)
    {
        logger.LogInformation($"Content: {content}");
        var decryptedContent = await encryptionService.Decrypt(content, masterPassword);
        
        logger.LogInformation($"{decryptedContent}");
        return decryptedContent;
    }
}