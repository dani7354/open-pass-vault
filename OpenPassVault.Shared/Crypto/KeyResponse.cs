namespace OpenPassVault.Shared.Crypto;

public record KeyResponse(byte[] Key, byte[] Salt)
{
    public const char Delimiter = '$';
    
    public string KeyHex => Convert.ToHexString(Key);
    private string SaltHex => Convert.ToHexString(Salt);
    public string KeyBase64 => Convert.ToBase64String(Key);
    public string SaltBase64 => Convert.ToBase64String(Salt);
    public string FullHexDigest => $"{KeyHex}${SaltHex}";
}
