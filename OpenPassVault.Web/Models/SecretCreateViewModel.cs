using System.ComponentModel.DataAnnotations;
using OpenPassVault.Shared.Validation;

namespace OpenPassVault.Web.Models;

public class SecretCreateViewModel
{
    [MaxLength(SecretFieldLengths.NameMaxLength), Required] 
    public string Name { get; set; } = null!;

    [MaxLength(SecretFieldLengths.TypeMaxLength), Required] 
    public string Type { get; set; } = null!;

    [MaxLength(SecretFieldLengths.ContentPlainTextMaxLength), Required] 
    public string ContentPlain { get; set; } = null!;
    
    [MaxLength(SecretFieldLengths.UsernameMaxLength)] 
    public string? Username { get; set; }

    [MaxLength(SecretFieldLengths.DescriptionMaxLength)]
    public string? Description { get; set; }
}