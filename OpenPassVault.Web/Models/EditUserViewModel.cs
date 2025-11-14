using System.ComponentModel.DataAnnotations;
using OpenPassVault.Shared.Validation;
using OpenPassVault.Shared.Validation.Attributes;

namespace OpenPassVault.Web.Models;

public class EditUserViewModel
{
    [Required(ErrorMessage = ErrorMessages.Required)]
    public string Id { get; set; } = null!;
    
    [Required(ErrorMessage = ErrorMessages.Required),
     EmailAddress(ErrorMessage = ErrorMessages.EmailAddress),
     MaxLength(RegisterFieldLengths.EmailMaxLength, ErrorMessage = ErrorMessages.MaxLength)]
    [UserEmailValidChars]
    public string Email { get; set; } = null!;
    
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
    MaxLength(RegisterFieldLengths.MasterPasswordMaxLength, ErrorMessage = ErrorMessages.MaxLength)]
    public string? NewMasterPassword { get; set; }
    
    [Compare(nameof(NewMasterPassword), ErrorMessage = ErrorMessages.Compare)]
    public string? ConfirmNewMasterPassword { get; set; }

    [Required(ErrorMessage = ErrorMessages.Required)]
    public string CaptchaCode { get; set; } = null!;
 
    public string CaptchaHmac { get; set; } = null!;
    
    public string CaptchaImageSrc { get; init; } = null!;
}