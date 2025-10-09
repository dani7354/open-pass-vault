using OpenPassVault.API.Services.Interfaces;
using OpenPassVault.API.Shared;
using OpenPassVault.Shared.Auth;
using OpenPassVault.Shared.Helpers;

namespace OpenPassVault.API.Middleware;

public class CsrfProtection(ICsrfTokenService csrfTokenService) : IMiddleware
{
    private static readonly HashSet<string> StateChangingMethods = [HttpMethods.Post, HttpMethods.Put, HttpMethods.Delete];
    
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var headers = context.Request.Headers;
        
        if (StateChangingMethods.Contains(context.Request.Method) 
            && headers.TryGetValue(Constants.Headers.Authorization, out var authorization))
        {
            var token = authorization.ToString().Replace("Bearer ", "");
            if (string.IsNullOrEmpty(token))
            {
                await Return403Forbidden(context);
                return;
            }

            var parsedToken = JwtParser.ToClaimsPrincipal(token);
            var sessionId = parsedToken.Claims.FirstOrDefault(x => x.Type == JwtClaimType.SessionId)?.Value;
            if (string.IsNullOrEmpty(sessionId))
            {
                await Return403Forbidden(context);
                return;
            }
            
            if (TryGetCsrfToken(context, out var csrfToken))
            {
                if (! await csrfTokenService.ValidateToken(sessionId, csrfToken))
                {
                    await Return403Forbidden(context);
                    return;
                }
            }
            else
            {
                await Return403Forbidden(context);
                return;
            }
        }
        
        await next.Invoke(context);
    }
    
    private bool TryGetCsrfToken(HttpContext context, out string csrfToken)
    {
        csrfToken = string.Empty;
        if (context.Request.Headers.TryGetValue(Constants.Headers.CsrfToken, out var csrfTokenFromHeader))
            csrfToken = csrfTokenFromHeader.ToString();

        else if (context.Request.Cookies.TryGetValue(Constants.Cookies.CsrfToken, out var csrfCookie))
            csrfToken = csrfCookie;

        return !string.IsNullOrEmpty(csrfToken);
    }
    
    private async Task Return403Forbidden(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        await context.Response.WriteAsync("Forbidden: CSRF token missing or invalid.");
    }
}

public static class CsrfProtectionExtensions
{
    public static IApplicationBuilder UseCsrfProtection(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CsrfProtection>();
    }
}