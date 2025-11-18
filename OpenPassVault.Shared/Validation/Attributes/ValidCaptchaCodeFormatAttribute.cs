using System.ComponentModel.DataAnnotations;
using OpenPassVault.Shared.Services.Interfaces;

namespace OpenPassVault.Shared.Validation.Attributes;

public class ValidCaptchaCodeFormatAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var valueStr = value?.ToString();
        if (!string.IsNullOrEmpty(valueStr))
        {
            var captchaService = (ICaptchaService)validationContext.GetService(typeof(ICaptchaService))!;
            if (captchaService == null)
                throw new InvalidOperationException("ICaptchaService not available in ValidationContext.");
            
            if (!captchaService.CaptchaFormatIsValid(valueStr))
                return new ValidationResult("Forkert format for captcha kode.");
        }

        return null;
    }
}