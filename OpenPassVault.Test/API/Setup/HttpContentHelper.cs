using System.Text.Json;

namespace OpenPassVault.Test.API.Setup;

public static class HttpContentHelper
{
    public static StringContent CreateStringContent<T>(T obj)
    {
        var json = JsonSerializer.Serialize(obj);
        return new StringContent(
            json, 
            System.Text.Encoding.UTF8, 
            mediaType: "application/json");
    }
}