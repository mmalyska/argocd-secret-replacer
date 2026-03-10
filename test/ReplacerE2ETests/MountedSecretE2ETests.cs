namespace ReplacerE2ETests;

using System.IO;
using System.Threading.Tasks;
using Utils;
using Xunit;

public class MountedSecretE2ETests
{
    [Fact]
    public async Task TestTokenReplacement()
    {
        var mountDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(mountDir);
        File.WriteAllText(Path.Combine(mountDir, "private-domain"), "PRIVATE_DOMAIN");

        const string inputText = "host: argocd.<secret:private-domain>";
        const string expectedOutput = "host: argocd.PRIVATE_DOMAIN";

        using var consoleOutput = new ConsoleOutput();
        using var consoleInput = ConsoleInput.FromString(inputText);

        var entryPoint = typeof(Program).Assembly.EntryPoint!;
        var options = new[] { "secret", $"--mount={mountDir}" };
        var returnObject = entryPoint.Invoke(null, new object[] { options });
        if (returnObject is Task returnTask)
            await returnTask;

        Assert.Equal(expectedOutput, consoleOutput.GetOutput());
        Directory.Delete(mountDir, true);
    }

    [Fact]
    public async Task TestBase64TokenReplacement()
    {
        var mountDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(mountDir);
        File.WriteAllText(Path.Combine(mountDir, "my-password"), "hunter2");

        // Base64 of "hunter2" = "aHVudGVyMg=="
        const string inputText = "password: <secret:my-password|base64>";
        const string expectedOutput = "password: aHVudGVyMg==";

        using var consoleOutput = new ConsoleOutput();
        using var consoleInput = ConsoleInput.FromString(inputText);

        var entryPoint = typeof(Program).Assembly.EntryPoint!;
        var options = new[] { "secret", $"--mount={mountDir}" };
        var returnObject = entryPoint.Invoke(null, new object[] { options });
        if (returnObject is Task returnTask)
            await returnTask;

        Assert.Equal(expectedOutput, consoleOutput.GetOutput());
        Directory.Delete(mountDir, true);
    }
}
