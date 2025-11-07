using System.ComponentModel.DataAnnotations;
using OpenPassVault.Shared.Validation;

namespace OpenPassVault.Web.Models;

public class DeleteUserViewModel
{
    [Required(ErrorMessage = ErrorMessages.Required)]
    public string UserId { get; set; } = null!;
    
    [Required(ErrorMessage = ErrorMessages.Required)]
    public string Password { get; set; } = null!;
    
    [Required(ErrorMessage = ErrorMessages.Required)]
    public string CaptchaCode { get; set; } = null!;
    
    public string CaptchaHmac { get; set; } = null!;
    
    public string CaptchaImageSrc { get; init; } = null!;
}