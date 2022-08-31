namespace ReplacerUnitTests;

using Common;
using Moq;
using Replacer.Modifiers;
using Replacer.SecretsProvider;
using Replacer.Substitution;
using Xunit;

public class ReplacerTests
{
    private SecretReplacer replacer;
    private readonly Mock<IModifiersFactory> modifiersFactoryMock;

    public ReplacerTests()
    {
        ISecretsProvider emptySecretsProvider = new SimpleSecretsProvider("");
        modifiersFactoryMock = new Mock<IModifiersFactory>();
        modifiersFactoryMock.Setup(a => a.GetModifier(It.IsAny<string>()))
            .Returns((IModifier?)null);
        replacer = new SecretReplacer(emptySecretsProvider, modifiersFactoryMock.Object);
    }

    [Fact]
    public void EmptyStringShouldReturnEmpty()
    {
        const string simpleString = "";

        var result = replacer.Replace(simpleString);

        Assert.Equal(simpleString, result);
    }

    [Fact]
    public void StringWithoutMatchShouldNotChange()
    {
        const string simpleString = "aaaa something aaaa";

        var result = replacer.Replace(simpleString);

        Assert.Equal(simpleString, result);
    }

    [Fact]
    public void MultilineStringWithoutMatchShouldNotChange()
    {
        const string simpleString = @"aaaa something aaaa
        bbbb different line

        ";

        var result = replacer.Replace(simpleString);

        Assert.Equal(simpleString, result);
    }

    [Fact]
    public void StringWithMatchShouldReplace()
    {
        const string simpleString = "aa<secret:path>bb";
        const string expectedString = "aa[replaced]bb";
        var secretProvider = new SimpleSecretsProvider("[replaced]");
        replacer = new SecretReplacer(secretProvider, modifiersFactoryMock.Object);

        var result = replacer.Replace(simpleString);

        Assert.Equal(expectedString, result);
    }

    [Fact]
    public void StringWithBase64ModifierShouldReturnEncodedValue()
    {
        const string simpleString = "aa<secret:path|base64>bb";
        const string expectedString = "aaW3JlcGxhY2VkXQ==bb";
        var secretProvider = new SimpleSecretsProvider("[replaced]");
        modifiersFactoryMock.Setup(a => a.GetModifier(It.IsAny<string>()))
            .Returns(new Base64Modifier());
        replacer = new SecretReplacer(secretProvider, modifiersFactoryMock.Object);

        var result = replacer.Replace(simpleString);

        Assert.Equal(expectedString, result);
    }

    [Fact]
    public void Base64StringShouldReturnReplacedEncodedValue()
    {
        const string simpleString = "PHNlY3JldDpwYXRoPg==";
        const string expectedString = "W3JlcGxhY2VkXQ==";
        var secretProvider = new SimpleSecretsProvider("[replaced]");
        modifiersFactoryMock.Setup(a => a.GetModifier(It.IsAny<string>()))
            .Returns(new Base64Modifier());
        replacer = new SecretReplacer(secretProvider, modifiersFactoryMock.Object);

        var result = replacer.Replace(simpleString);

        Assert.Equal(expectedString, result);
    }

    [Fact]
    public void Base64StringShouldReturnSameEncodedValue()
    {
        const string simpleString = "YWF3ZGFzZA==";
        const string expectedString = "YWF3ZGFzZA==";
        var secretProvider = new SimpleSecretsProvider("[replaced]");
        modifiersFactoryMock.Setup(a => a.GetModifier(It.IsAny<string>()))
            .Returns(new Base64Modifier());
        replacer = new SecretReplacer(secretProvider, modifiersFactoryMock.Object);

        var result = replacer.Replace(simpleString);

        Assert.Equal(expectedString, result);
    }
}
