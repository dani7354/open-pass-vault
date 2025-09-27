using OpenPassVault.Shared.DTO;
using OpenPassVault.Shared.Validation;

namespace OpenPassVault.Test.Shared;

public class ValidSecretTypeAttributeTest
{

    public static IList<object[]> ValidSecretTypes = Enum.GetValues<SecretType>().Select(x => new object[] { x }).ToArray();

    [Theory]
    [MemberData(nameof(ValidSecretTypes))]
    public void IsValid_OnValidStringInput_PassesValidation(SecretType secretType)
    {
        var attribute = new ValidSecretTypeAttribute();

        var isValid = attribute.IsValid(secretType.ToString());
        
        Assert.True(isValid);
    }
}