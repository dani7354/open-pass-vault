using Microsoft.Net.Http.Headers;

namespace OpenPassVault.API.Middleware;

public class SecurityHeaders : IMiddleware
{
    private const string HstsValue = "max-age=31536000; includeSubDomains; preload";
    private const string XContentTypeOptionsValue = "nosniff";
    private const string XFrameOptionsValue = "DENY";
    
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        context.Response.Headers[HeaderNames.StrictTransportSecurity] = HstsValue;
        context.Response.Headers[HeaderNames.XContentTypeOptions] = XContentTypeOptionsValue;
        context.Response.Headers[HeaderNames.XFrameOptions] = XFrameOptionsValue;
        
        await next.Invoke(context);
    }
}

public static class SecurityHeadersExtensions
{
    public static IApplicationBuilder UseSecurityHeaders(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SecurityHeaders>();
    }
}