using System.ComponentModel.DataAnnotations;

namespace OpenPassVault.Web.Models;

public class RegisterViewModel
{
    [Required, EmailAddress, MaxLength(256)]
    public string Email { get; set; } = null!;

    [Required, MinLength(12), MaxLength(256)]
    public string Password { get; set; } = null!;

    [Required, Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = null!;

    [Required, MinLength(16), MaxLength(256)]
    public string MasterPassword { get; set; } = null!;

    [Required, Compare(nameof(MasterPassword))]
    public string ConfirmMasterPassword { get; set; } = null!;
}