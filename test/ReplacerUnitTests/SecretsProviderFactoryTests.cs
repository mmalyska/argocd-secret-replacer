namespace ReplacerUnitTests;

using System;
using Replacer;
using Replacer.SecretsProvider;
using Replacer.SecretsProvider.Sops;
using Xunit;

public class SecretsProviderFactoryTests
{
    [Fact]
    public void WhenRequestedSopsShouldReturnProvider()
    {
        var options = new SopsOptions { File = "testFile" };
        var factory = new SecretsProviderFactory();
        var provider = factory.GetProvider(options);

        Assert.IsType<SopsSecretProvider>(provider);
    }

    [Fact]
    public void WhenRequestedProviderNotFoundShouldThrow()
    {
        var factory = new SecretsProviderFactory();
        Assert.Throws<ArgumentOutOfRangeException>(() => factory.GetProvider(new object()));
    }
}
