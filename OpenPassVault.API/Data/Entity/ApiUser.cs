using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace OpenPassVault.API.Data.Entity;

public class ApiUser : IdentityUser
{
    [MaxLength(100)]
    public string MasterPasswordHash { get; set; } = string.Empty;
}