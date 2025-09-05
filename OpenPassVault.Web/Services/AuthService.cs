using OpenPassVault.Web.Services.Interfaces;

namespace OpenPassVault.Web.Services;

public class AuthService(IHttpApiService httpApiService) : IAuthService
{
    private IHttpApiService _httpApiService = httpApiService;
}
