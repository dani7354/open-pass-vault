namespace OpenPassVault.Shared.DTO;

public class SecretUpdateBatchRequest
{
    public IList<SecretUpdateRequest> Secrets { get; set; } = new List<SecretUpdateRequest>();
}
