using System.ComponentModel.DataAnnotations;

namespace OpenPassVault.Shared.Validation.Attributes;

public class ValidBase64StringAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        var valueStr = value?.ToString();
        if (!string.IsNullOrEmpty(valueStr))
        {
            try
            {
                _ = Convert.FromBase64String(valueStr);
                return true;
            }
            catch (FormatException)
            {
                ErrorMessage = "Strengen er ikke en gyldig Base64-kodet streng";
                return false;
            }
        }

        return true;
    }
}