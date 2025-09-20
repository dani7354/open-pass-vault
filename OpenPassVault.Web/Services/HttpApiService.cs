using System.Net;
using System.Text.Json;
using OpenPassVault.Web.Services.Interfaces;
using System.Net.Http.Headers;

namespace OpenPassVault.Web.Services;

public class HttpApiService : IHttpApiService
{
    private const string ContentType = "application/json";
    private const string AuthScheme = "Bearer";
    
    private readonly HttpClient _client = new();
    private readonly IAccessTokenService _accessTokenService;
    
    
    public HttpApiService(IAccessTokenService accessTokenService, string baseUrl)
    {
        _client.BaseAddress = new Uri(baseUrl);
        _accessTokenService = accessTokenService;
    }
    
    public async Task<T?> GetAsync<T>(string url)
    {
        try
        {
            AddAuthorizationHeaderIfExists();
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            return string.IsNullOrEmpty(responseContent) ? default : JsonSerializer.Deserialize<T>(responseContent);

        }
        catch (HttpRequestException e)
        {
            if (e.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException();
            }
            throw;
        }
    }

    public async Task<T?> PostAsync<T>(string url, object data)
    {
        try
        {
            AddAuthorizationHeaderIfExists();
            var response = await _client.PostAsync(url, CreateContent(data));
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
        
            return string.IsNullOrEmpty(responseContent) ? default : JsonSerializer.Deserialize<T>(responseContent);
        }
        catch (HttpRequestException e)
        {
            if (e.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException();
            }
            throw;
        }
    }

    private HttpContent CreateContent(object data)
    {
        var content = new StringContent(JsonSerializer.Serialize(data));
        content.Headers.ContentType = new MediaTypeHeaderValue(ContentType);
        return content;
    }

    private void AddAuthorizationHeaderIfExists()
    {
        var token = _accessTokenService.GetToken(); 
        if (!string.IsNullOrEmpty(token) && 
            (_client.DefaultRequestHeaders.Authorization == null || 
             _client.DefaultRequestHeaders.Authorization.Parameter != token))
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthScheme, token);
        }
    }
}