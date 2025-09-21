namespace OpenPassVault.Web.Services.Exceptions;

public class AuthenticationException(string message) : Exception(message)
{
    public AuthenticationException() : this("Authentication failed.") { }
}