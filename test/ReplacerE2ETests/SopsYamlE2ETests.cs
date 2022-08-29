namespace ReplacerE2ETests;

using System.IO;
using System.Threading.Tasks;
using Utils;
using Xunit;

[Collection("SopsEnv")]
public class SopsYamlE2ETests
{
    [Fact]
    public async Task TestSimpleReplacement()
    {
        const string inputText = "A<secret:secretKey1>B";
        const string expectedOutput = "AsecretValue1B";
        using var consoleOutput = new ConsoleOutput();
        using var consoleInput = ConsoleInput.FromString(inputText);

        var entryPoint = typeof(Program).Assembly.EntryPoint!;
        var options = new[] { "sops", $"-f sops{Path.DirectorySeparatorChar}sops.sec.yaml" };
        var returnObject = entryPoint.Invoke(null, new object[] { options });
        if (returnObject is Task returnTask)
        {
            await returnTask;
        }

        Assert.Equal(expectedOutput, consoleOutput.GetOutput());
    }
}
