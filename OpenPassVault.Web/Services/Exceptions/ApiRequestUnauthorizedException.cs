namespace OpenPassVault.Web.Services.Exceptions;

public class ApiRequestUnauthorizedException(string message) : Exception(message);