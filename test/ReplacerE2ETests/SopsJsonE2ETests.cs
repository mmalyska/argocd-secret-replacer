namespace ReplacerE2ETests;

using System;
using System.Threading.Tasks;
using ReplacerE2ETests.Utils;
using Xunit;

[Collection("SopsEnv")]
public class SopsJsonE2ETests
{
    [Fact]
    public async Task TestSimpleReplacement()
    {
        string inputText = "A<secret:secretKey1>B";
        string expectedOutput = "AsecretValue1B";
        using var consoleOutput = new ConsoleOutput();
        using var consoleInput = ConsoleInput.FromString(inputText);

        var entryPoint = typeof(Program).Assembly.EntryPoint!;
        var options = new string[]{
            "sops",
            "-f sops/sops.sec.json",
        };
        var returnObject = entryPoint.Invoke(null, new object[] { options });
        if(returnObject is Task returnTask)
        {
            await returnTask;
        }

        Assert.Equal(expectedOutput, consoleOutput.GetOuput());
    }
}
