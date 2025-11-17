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
            
            try
            {
                _ = Guid.Parse(valueStr);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        return true;    
    }
}