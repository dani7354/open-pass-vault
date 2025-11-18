using System.ComponentModel.DataAnnotations;

namespace OpenPassVault.Shared.Validation.Attributes;

public class ValidGuidFormatAttribute : ValidationAttribute
{
    private const int GuidLength = 36;
    
    public override bool IsValid(object? value)
    {
        var valueStr = value?.ToString();
        if (!string.IsNullOrEmpty(valueStr))
        {
            if (valueStr.Length != GuidLength)
                return false;
            
            if (!Guid.TryParse(valueStr, out _))
                return false;

            return true;
        }

        return true;    
    }
}