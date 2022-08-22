using Xunit;

namespace ReplacerUnitTests;

using System;
using Common;
using replacer.SecretsProvider;
using replacer.Substitution;

public class ReplacerTests
{
    private readonly Replacer replacer;
    private readonly ISecretsProvider emptySecretsProvider;

    public ReplacerTests()
    {
        emptySecretsProvider = new SimpleSecretsProvider("");
        replacer = new Replacer(emptySecretsProvider);
    }

    [Fact]
    public void EmptyStringShouldReturnEmpty()
    {
        const string  simpleString = "";

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
        var simpleString = "aaaa something aaaa" + Environment.NewLine + "bbbb different line";

        var result = replacer.Replace(simpleString);

        Assert.Equal(simpleString, result);
    }

    [Fact]
    public void StringWithExpressionShouldReplace()
    {
        var simpleString = "aa<secret:path>bb";
        var expectedString = "aa[replaced]bb";
        var secretProvider = new SimpleSecretsProvider("[replaced]");

        var result = replacer.Replace(simpleString);

        Assert.Equal(expectedString, result);
    }
}
