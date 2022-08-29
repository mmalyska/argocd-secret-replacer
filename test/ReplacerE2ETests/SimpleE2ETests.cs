namespace ReplacerE2ETests;

using System;
using System.Threading.Tasks;
using Utils;
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
        var options = new[] { "sops" };
        var returnObject = entryPoint.Invoke(null, new object[] { options });
        if (returnObject is Task returnTask)
        {
            await returnTask;
        }

        Assert.Equal(inputText, consoleOutput.GetOutput());
    }

    [Fact]
    public async Task TestSimpleMultiline()
    {
        var inputText = "B" + Environment.NewLine + "C";
        using var consoleOutput = new ConsoleOutput();
        using var consoleInput = ConsoleInput.FromString(inputText);

        var entryPoint = typeof(Program).Assembly.EntryPoint!;
        var options = new[] { "sops" };
        var returnObject = entryPoint.Invoke(null, new object[] { options });
        if (returnObject is Task returnTask)
        {
            await returnTask;
        }

        Assert.Equal(inputText, consoleOutput.GetOutput());
    }
}
