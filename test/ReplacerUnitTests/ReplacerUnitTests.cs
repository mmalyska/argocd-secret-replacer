namespace ReplacerUnitTests;

using Common;
using Replacer.SecretsProvider;
using Replacer.Substitution;
using Xunit;

public class ReplacerTests
{
    private readonly ISecretsProvider emptySecretsProvider;
    private SecretReplacer replacer;

    public ReplacerTests()
    {
        emptySecretsProvider = new SimpleSecretsProvider("");
        replacer = new SecretReplacer(emptySecretsProvider);
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
        var simpleString = @"aaaa something aaaa
        bbbb different line
        
        ";

        var result = replacer.Replace(simpleString);

        Assert.Equal(simpleString, result);
    }

    [Fact]
    public void StringWithExpressionShouldReplace()
    {
        var simpleString = "aa<secret:path>bb";
        var expectedString = "aa[replaced]bb";
        var secretProvider = new SimpleSecretsProvider("[replaced]");
        replacer = new SecretReplacer(secretProvider);

        var result = replacer.Replace(simpleString);

        Assert.Equal(expectedString, result);
    }
}
