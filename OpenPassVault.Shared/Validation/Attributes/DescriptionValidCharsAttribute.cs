namespace OpenPassVault.Shared.Validation.Attributes;

public class DescriptionValidCharsAttribute() 
    : SpecialCharactersWhitelistAttribute(ValidSpecialCharacters)
{
    private static readonly HashSet<char> ValidSpecialCharacters = [
        '.', ',', ';', ':', '-', '_', '(', ')', '=', '"', '\'', '/', '#', '@', ' '];
}