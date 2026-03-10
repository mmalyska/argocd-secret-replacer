namespace ReplacerUnitTests;

using System;
using System.IO;
using Replacer;
using Replacer.SecretsProvider;
using Replacer.SecretsProvider.MountedSecret;
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

    [Fact]
    public void WhenMountedSecretOptionsShouldReturnMountedSecretProvider()
    {
        var dir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(dir);

        var factory = new SecretsProviderFactory();
        var opts = new MountedSecretOptions { MountPath = dir };
        var provider = factory.GetProvider(opts);

        Assert.IsType<MountedSecretProvider>(provider);
    }
}
