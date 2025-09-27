using System.ComponentModel.DataAnnotations;
using OpenPassVault.Shared.Validation;
using OpenPassVault.Shared.Validation.Attributes;

namespace OpenPassVault.Shared.DTO;

public class SecretCreateRequest
{
    [MaxLength(SecretFieldLengths.NameMaxLength), Required]
    [NameValidChars]
    public string Name { get; set; } = null!;

    [MaxLength(SecretFieldLengths.TypeMaxLength), Required]
    [ValidSecretType]
    public string Type { get; set; } = null!;

    [MaxLength(SecretFieldLengths.ContentMaxLength), Required] 
    public string Content { get; set; } = null!;
    
    [MaxLength(SecretFieldLengths.UsernameMaxLength)]
    [UsernameValidChars]
    public string? Username { get; set; }

    [MaxLength(SecretFieldLengths.DescriptionMaxLength)]
    [DescriptionValidChars]
    public string? Description { get; set; }
}