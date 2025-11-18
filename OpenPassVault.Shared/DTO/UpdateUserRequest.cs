using System.ComponentModel.DataAnnotations;
using OpenPassVault.Shared.Validation;
using OpenPassVault.Shared.Validation.Attributes;

namespace OpenPassVault.Shared.DTO;

public class UpdateUserRequest
{
    [Required(ErrorMessage = ErrorMessages.Required)]
    [ValidGuidFormat]
    public string Id { get; set; } = null!;
    
    [Required(ErrorMessage = ErrorMessages.Required),
     MinLength(RegisterFieldLengths.PasswordMinLength, ErrorMessage = ErrorMessages.MinLength),
     MaxLength(RegisterFieldLengths.PasswordMaxLength, ErrorMessage = ErrorMessages.MaxLength)]
    public string CurrentPassword { get; set; } = string.Empty;
    
    [MinLength(RegisterFieldLengths.PasswordMinLength, ErrorMessage = ErrorMessages.MinLength),
     MaxLength(RegisterFieldLengths.PasswordMaxLength, ErrorMessage = ErrorMessages.MaxLength)]
    public string? NewPassword { get; set; }
    
    [Compare(nameof(NewPassword), ErrorMessage = ErrorMessages.Compare)]
    public string? ConfirmNewPassword { get; set; }
    
    [MinLength(RegisterFieldLengths.MasterPasswordMinLength, ErrorMessage = ErrorMessages.MinLength),
     MaxLength(RegisterFieldLengths.MasterPasswordHashMaxLength, ErrorMessage = ErrorMessages.MaxLength)]
    public string? MasterPasswordHash { get; set; }

    [Required(ErrorMessage = ErrorMessages.Required)]
    [ValidCaptchaCodeFormat]
    public string CaptchaCode { get; set; } = null!;
 
    [Required(ErrorMessage = ErrorMessages.Required)]
    public string CaptchaHmac { get; set; } = null!;
}