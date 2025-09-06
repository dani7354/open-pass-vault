namespace OpenPassVault.Shared.Crypto;

public record KeyResponse(byte[] Key, byte[] Salt)
{
    public const char Delimiter = '$';
    
    public string KeyHex => Convert.ToHexString(Key);
    private string SaltHex => Convert.ToHexString(Salt);
    public string FullDigest => $"{KeyHex}${SaltHex}";
}
