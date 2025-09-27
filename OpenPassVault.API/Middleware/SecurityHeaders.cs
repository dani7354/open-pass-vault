namespace OpenPassVault.API.Middleware;

public class SecurityHeaders : IMiddleware
{
    private const string HstsHeader = "Strict-Transport-Security";
    private const string HstsValue = "max-age=31536000; includeSubDomains; preload";
    
    private const string ContentTypeOptionsHeader = "X-Content-Type-Options";
    private const string ContentTypeOptionsValue = "nosniff";
    
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        //context.Response.Headers[HstsHeader] = HstsValue;
        context.Response.Headers[ContentTypeOptionsHeader] = ContentTypeOptionsValue;
        
        await next.Invoke(context);
    }
}