namespace OpenPassVault.Web.Services.Interfaces;

public interface IHttpApiService
{
    Task<T?> GetAsync<T>(string url);
    Task<T?> PostAsync<T>(string url, object data);
    Task<T?> PutAsync<T>(string url, object data);
    Task DeleteAsync(string url);
}