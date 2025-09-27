using System.ComponentModel.DataAnnotations;
using OpenPassVault.Shared.Validation;
using OpenPassVault.Shared.Validation.Attributes;

namespace OpenPassVault.Web.Models;

public class SecretCreateViewModel
{
    [MaxLength(SecretFieldLengths.NameMaxLength), Required]
    [ValidSecretType]
    public string Name { get; set; } = null!;

    [MaxLength(SecretFieldLengths.TypeMaxLength), Required]
    [ValidSecretType]
    public string Type { get; set; } = null!;

    [MaxLength(SecretFieldLengths.ContentPlainTextMaxLength), Required]
    public string ContentPlain { get; set; } = null!;
    
    [MaxLength(SecretFieldLengths.UsernameMaxLength)]
    [OnlyAllowedSpecialCharacters]
    public string? Username { get; set; }

    [MaxLength(SecretFieldLengths.DescriptionMaxLength)]
    [OnlyAllowedSpecialCharacters]
    public string? Description { get; set; }
}