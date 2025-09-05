namespace OpenPassVault.Web.Services.Interfaces;

public interface IHttpApiService
{
    Task<T?> GetAsync<T>(string url);
    Task PostAsync<T>(string url, T data);
}