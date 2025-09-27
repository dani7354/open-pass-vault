using System.ComponentModel.DataAnnotations;
using OpenPassVault.Shared.Validation;

namespace OpenPassVault.Shared.DTO;

public class SecretCreateRequest
{
    [MaxLength(SecretFieldLengths.NameMaxLength), Required]
    [OnlyAllowedSpecialCharacters]
    public string Name { get; set; } = null!;

    [MaxLength(SecretFieldLengths.TypeMaxLength), Required]
    [ValidSecretType]
    public string Type { get; set; } = null!;

    [MaxLength(SecretFieldLengths.ContentMaxLength), Required] 
    public string Content { get; set; } = null!;
    
    [MaxLength(SecretFieldLengths.UsernameMaxLength)]
    [OnlyAllowedSpecialCharacters]
    public string? Username { get; set; }

    [MaxLength(SecretFieldLengths.DescriptionMaxLength)]
    [OnlyAllowedSpecialCharacters]
    public string? Description { get; set; }
}