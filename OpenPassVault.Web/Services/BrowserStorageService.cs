using System.Text.Json;
using Microsoft.JSInterop;
using OpenPassVault.Web.Services.Interfaces;

namespace OpenPassVault.Web.Services;

public class BrowserStorageService(IJSRuntime jsRuntime) : IBrowserStorageService
{
    public async Task<T?> GetItem<T>(string key)
    {
        var json = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", key);
        return json == null ? default : JsonSerializer.Deserialize<T>(json);
    }

    public async Task SetItem<T>(string key, T value)
    {
        await jsRuntime.InvokeVoidAsync("localStorage.setItem", key, JsonSerializer.Serialize(value));
    }

    public async Task RemoveItem(string key)
    {
        await jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
    }
}