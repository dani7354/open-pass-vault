using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using OpenPassVault.Shared.Validation;

namespace OpenPassVault.API.Data.Entity;

public class Secret
{
    [MaxLength(SecretFieldLengths.IdMaxLength), Key] 
    public string Id { get; init; } = null!;

    [MaxLength(SecretFieldLengths.IdMaxLength)]
    public string UserId { get; init; } = null!;

    [MaxLength(SecretFieldLengths.NameMaxLength), Required] 
    public string Name { get; set; } = null!;
    
    [MaxLength(SecretFieldLengths.DescriptionMaxLength)] 
    public string? Username { get; set; }
    
    [MaxLength(SecretFieldLengths.DescriptionMaxLength)]
    public string? Description { get; set; }

    [MaxLength(SecretFieldLengths.ContentMaxLength), Required] 
    public string Content { get; set; } = null!;

    [MaxLength(SecretFieldLengths.TypeMaxLength), Required]
    public string Type { get; set; } = null!;
    
    public DateTime Created { get; init; }
    
    public DateTime Updated { get; set; }

    [ForeignKey(nameof(UserId))]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public ApiUser User { get; } = null!;
}