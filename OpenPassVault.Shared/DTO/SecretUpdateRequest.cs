using System.ComponentModel.DataAnnotations;
using OpenPassVault.Shared.Validation;

namespace OpenPassVault.Shared.DTO;

public class SecretUpdateRequest
{
    [MaxLength(SecretFieldLengths.IdMaxLength), Required]
    public string Id { get; set; } = null!;
    
    [MaxLength(SecretFieldLengths.NameMaxLength), Required] 
    public string Name { get; set; } = null!;

    [MaxLength(SecretFieldLengths.TypeMaxLength), Required] 
    public string Type { get; set; } = null!;

    [MaxLength(SecretFieldLengths.ContentMaxLength), Required] 
    public string Content { get; set; } = null!;
    
    [MaxLength(SecretFieldLengths.UsernameMaxLength)] 
    public string? Username { get; set; }

    [MaxLength(SecretFieldLengths.DescriptionMaxLength)]
    public string? Description { get; set; }
}