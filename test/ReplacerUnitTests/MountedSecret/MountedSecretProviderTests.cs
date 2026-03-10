namespace ReplacerUnitTests.MountedSecret;

using System.IO;
using Replacer;
using Replacer.SecretsProvider.MountedSecret;
using Xunit;

public class MountedSecretProviderTests
{
    [Fact]
    public void WhenKeyExistsShouldReturnValue()
    {
        var dir = CreateTempSecret(("private-domain", "PRIVATE_DOMAIN"));
        var provider = new MountedSecretProvider(new MountedSecretOptions { MountPath = dir });

        Assert.Equal("PRIVATE_DOMAIN", provider.GetSecret("private-domain"));
    }

    [Fact]
    public void WhenKeyMissingShouldReturnEmpty()
    {
        var dir = CreateTempSecret(("other-key", "value"));
        var provider = new MountedSecretProvider(new MountedSecretOptions { MountPath = dir });

        Assert.Equal(string.Empty, provider.GetSecret("missing-key"));
    }

    [Fact]
    public void WhenValueHasTrailingNewlineShouldTrim()
    {
        var dir = CreateTempSecret(("my-key", "hello\n"));
        var provider = new MountedSecretProvider(new MountedSecretOptions { MountPath = dir });

        Assert.Equal("hello", provider.GetSecret("my-key"));
    }

    [Fact]
    public void WhenDirectoryMissingShouldThrow()
    {
        var opts = new MountedSecretOptions { MountPath = "/nonexistent/path/abc123" };
        Assert.Throws<DirectoryNotFoundException>(() => new MountedSecretProvider(opts));
    }

    private static string CreateTempSecret(params (string key, string value)[] entries)
    {
        var dir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(dir);
        foreach (var (key, value) in entries)
            File.WriteAllText(Path.Combine(dir, key), value);
        return dir;
    }
}
