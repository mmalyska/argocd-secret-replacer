namespace ReplacerE2ETests;

using System;
using System.Threading.Tasks;
using ReplacerE2ETests.Utils;
using Xunit;

public class SimpleE2ETests
{
    [Fact]
    public async Task TestSimple()
    {
        const string inputText = "B";
        using var consoleOutput = new ConsoleOutput();
        using var consoleInput = ConsoleInput.FromString(inputText);

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
        using var consoleInput = ConsoleInput.FromString(inputText);

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
}
