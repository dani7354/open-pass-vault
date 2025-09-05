using System.Text.Json;
using OpenPassVault.Web.Services.Interfaces;

namespace OpenPassVault.Web.Services;

public class HttpApiService(string baseUrl) : IHttpApiService
{
    private readonly HttpClient _client = new() { BaseAddress = new Uri(baseUrl) };

    public async Task<T?> GetAsync<T>(string url)
    {
        var response = await _client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<T>(json);
    }

    public async Task PostAsync<T>(string url, T data)
    {
        var response = await _client.PostAsync(url, new StringContent(JsonSerializer.Serialize(data)));
        response.EnsureSuccessStatusCode();
    }
}