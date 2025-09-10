using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OpenPassVault.API.Data.Entity;

public class Secret
{
    [MaxLength(36), Key] 
    public string Id { get; set; } = null!;

    [MaxLength(255)]
    public string UserId { get; set; } = null!;

    [MaxLength(255), Required] 
    public string Name { get; set; } = null!;
    
    [MaxLength(512)]
    public string? Description { get; set; }

    [MaxLength(1024), Required] 
    public string Content { get; set; } = null!;

    [MaxLength(64), Required]
    public string Type { get; set; } = null!;
    
    public DateTime Created { get; set; }
    
    public DateTime Updated { get; set; }

    [ForeignKey(nameof(UserId))]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public ApiUser User { get; set; } = null!;
}