using System.Net;
using System.Text.Json;
using OpenPassVault.Web.Services.Interfaces;
using System.Net.Http.Headers;

namespace OpenPassVault.Web.Services;

public class HttpApiService(IAccessTokenStorage accessTokenStorage, HttpClient httpClient)
    : IHttpApiService
{
    private const string ContentType = "application/json";
    private const string AuthScheme = "Bearer";

    public async Task<T?> GetAsync<T>(string url)
    {
        try
        {
            await AddAuthorizationHeaderIfExists();
            var response = await httpClient.GetAsync(url);
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
            await AddAuthorizationHeaderIfExists();
            var response = await httpClient.PostAsync(url, CreateContent(data));
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

    public async Task DeleteAsync(string url)
    {
        try
        {
            await AddAuthorizationHeaderIfExists();
            var response = await httpClient.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
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

    private async Task AddAuthorizationHeaderIfExists()
    {
        var token = await accessTokenStorage.GetToken(); 
        if (!string.IsNullOrEmpty(token) && 
            (httpClient.DefaultRequestHeaders.Authorization == null || 
             httpClient.DefaultRequestHeaders.Authorization.Parameter != token))
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthScheme, token);
        }
    }
}