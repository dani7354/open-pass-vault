using OpenPassVault.Shared.DTO;
using OpenPassVault.Shared.Validation.Attributes;

namespace OpenPassVault.Test.Shared;

public class ValidSecretTypeAttributeTest
{
    public static IList<object[]> ValidSecretTypes = Enum.GetValues<SecretType>().Select(x => new object[] { x }).ToArray();
    public static IList<object[]> InValidSecretTypes = new List<object[]>
    {
        new object[] { "" },
        new object[] { " " },
        new object[] { "---- Vælg ----" },
        new object[] { "c4rd" },
        new object[] { "hello" },
        new object[] { "999" },
        new object[] { "-1" },
        new object[] { "2.5" },
    };

    [Theory]
    [MemberData(nameof(ValidSecretTypes))]
    public void IsValid_OnValidInputString_PassesValidation(SecretType secretType)
    {
        var attribute = new ValidSecretTypeAttribute();

        var isValidNameString = attribute.IsValid(secretType.ToString());
        var isValidValueString = attribute.IsValid(((int)secretType).ToString());
        
        Assert.True(isValidNameString && isValidValueString);
    }
    
    [Theory]
    [MemberData(nameof(InValidSecretTypes))]
    public void IsValid_OnInValidInputString_FailsValidation(string secretTypeValue)
    {
        var attribute = new ValidSecretTypeAttribute();

        var isValid = attribute.IsValid(secretTypeValue);
        
        Assert.False(isValid);
    }
    
    [Fact]
    public void IsValid_OnNullInput_FailsValidation()
    {
        var attribute = new ValidSecretTypeAttribute();

        var isValid = attribute.IsValid(null);
        
        Assert.False(isValid);
    }
}