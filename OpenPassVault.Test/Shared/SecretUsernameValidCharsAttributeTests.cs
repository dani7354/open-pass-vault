using OpenPassVault.Shared.Validation.Attributes;

namespace OpenPassVault.Test.Shared;

public class SecretUsernameValidCharsAttributeTests
{
    public static readonly IList<object[]> ValidInput =
    [
        new object[] { "username" },
        new object[] { "user.name" },
        new object[] { "user-name" },
        new object[] { "user_name" },
        new object[] { "UserName123" },
        new object[] { "USER.NAME-123" },
        new object[] { "user_name_456" },
    ];
    
    public static readonly IList<object[]> InvalidInput =
    [
        new object[] { "user!name" },
        new object[] { "user#name" },
        new object[] { "user$name" },
        new object[] { "user%name" },
        new object[] { "user^name" },
        new object[] { "user&name" },
        new object[] { "user*name" },
        new object[] { "user(name)" },
        new object[] { "user=name" },
        new object[] { "user{name}" },
        new object[] { "user[name]" },
        new object[] { "user|name" },
        new object[] { "user\\name" },
        new object[] { "user/name" },
        new object[] { "user<name>" },
        new object[] { "user,name" },
        new object[] { "user?name" },
        new object[] { "user~name" },
        new object[] { "user`name" },
        new object[] { "user😄name" },
        new object[] { "user name" },
        new object[] { "user\tname" },
        new object[] { "𝔲𝔰𝔢𝔯𝔫𝔞𝔪𝔢" },
        new object[] { "🖕🏿🖕🏿🖕🏿🖕🏿🖕🏿" },
    ];
    
    [Theory]
    [MemberData(nameof(ValidInput))]
    public void IsValid_OnValidInput_PassesValidation(string inputValue)
    {
        var attribute = new SecretUsernameValidCharsAttribute();

        var isValid = attribute.IsValid(inputValue);
        
        Assert.True(isValid);
    }
    
    [Theory]
    [MemberData(nameof(InvalidInput))]
    public void IsValid_OnInvalidInput_FailsValidation(string inputValue)
    {
        var attribute = new SecretUsernameValidCharsAttribute();

        var isValid = attribute.IsValid(inputValue);
        
        Assert.False(isValid);
    }
}