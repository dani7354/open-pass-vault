namespace OpenPassVault.Shared.Validation.Attributes;

public class NameValidCharsAttribute() 
    : SpecialCharactersWhitelistAttribute(AllowedSpecialCharacters)
{
    private static readonly HashSet<char> AllowedSpecialCharacters = [
        '.', ',', ';', ':', '-', '_', '(', ')', '#', '@', ' '
    ];
}