namespace ReplacerE2ETests;

using System;
using System.Threading.Tasks;
using Xunit;

public class SopsJsonE2ETests
{
    [Fact]
    public async Task TestSimple()
    {
        const string inputText = "B";
        using var consoleOutput = new ConsoleOutput();
        using var consoleInput = new ConsoleInput(inputText);

        var entryPoint = typeof(Program).Assembly.EntryPoint!;
        var options = new string[]{
            "sops"
        };
        var returnObject = entryPoint.Invoke(null, new object[] { options });
        if(returnObject is Task returnTask)
        {
            await returnTask;
        }

        Assert.Equal(inputText, consoleOutput.GetOuput());
    }

    [Fact]
    public async Task TestSimpleMultiline()
    {
        string inputText = "B" + Environment.NewLine + "C";
        using var consoleOutput = new ConsoleOutput();
        using var consoleInput = new ConsoleInput(inputText);

        var entryPoint = typeof(Program).Assembly.EntryPoint!;
        var options = new string[]{
            "sops"
        };
        var returnObject = entryPoint.Invoke(null, new object[] { options });
        if(returnObject is Task returnTask)
        {
            await returnTask;
        }

        Assert.Equal(inputText, consoleOutput.GetOuput());
    }

    [Fact]
    public async Task TestSimpleReplacement()
    {
        Environment.SetEnvironmentVariable("SOPS_AGE_KEY_FILE", "sops/key.txt");
        string inputText = "A<secret:secretKey1>B";
        string expectedOutput = "AsecretValue1B";
        using var consoleOutput = new ConsoleOutput();
        using var consoleInput = new ConsoleInput(inputText);

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
