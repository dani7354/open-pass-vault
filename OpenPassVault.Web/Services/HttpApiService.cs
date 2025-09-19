using System.Text.Json;
using OpenPassVault.Web.Services.Interfaces;
using System.Net.Http.Headers;

namespace OpenPassVault.Web.Services;

public class HttpApiService : IHttpApiService
{
    private const string ContentType = "application/json";
    private const string AuthScheme = "Bearer";
    
    private readonly HttpClient _client = new();
    private readonly IMemoryStorageService _memoryStorageService;
    
    public HttpApiService(IMemoryStorageService memoryStorageService, string baseUrl)
    {
        _client.BaseAddress = new Uri(baseUrl);
        _memoryStorageService = memoryStorageService;
    }
    
    public async Task<T?> GetAsync<T>(string url)
    {
        AddAuthorizationHeaderIfExists();
        var response = await _client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        
        return string.IsNullOrEmpty(responseContent) ? default : JsonSerializer.Deserialize<T>(responseContent);
    }

    public async Task<T?> PostAsync<T>(string url, object data)
    {
        AddAuthorizationHeaderIfExists();
        var response = await _client.PostAsync(url, CreateContent(data));
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        
        return string.IsNullOrEmpty(responseContent) ? default : JsonSerializer.Deserialize<T>(responseContent);
    }

    private HttpContent CreateContent(object data)
    {
        var content = new StringContent(JsonSerializer.Serialize(data));
        content.Headers.ContentType = new MediaTypeHeaderValue(ContentType);
        return content;
    }

    private void AddAuthorizationHeaderIfExists()
    {
        var token = _memoryStorageService.GetItem<string>("OpenPassVault.API");
        if (!string.IsNullOrEmpty(token) && 
            (_client.DefaultRequestHeaders.Authorization == null || 
             _client.DefaultRequestHeaders.Authorization.Parameter != token))
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthScheme, token);
        }
    }
}