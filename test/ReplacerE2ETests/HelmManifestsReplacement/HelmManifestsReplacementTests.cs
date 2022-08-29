namespace ReplacerE2ETests.HelmManifestsReplacement;

using System.Threading.Tasks;
using System.IO;
using ReplacerE2ETests.Utils;
using Xunit;

[Collection("SopsEnv")]
public class HelmManifestsReplacementTests
{
    [Fact]
    public async void TestReplacements(){
        string inputFile = $"HelmManifestsReplacement{Path.DirectorySeparatorChar}HelmManifestsReplacement.Manifest.yaml";
        string expectedFile = $"HelmManifestsReplacement{Path.DirectorySeparatorChar}HelmManifestsReplacement.Manifest.expected.yaml";
        using var consoleOutput = new ConsoleOutput();
        using var consoleInput = ConsoleInput.FromFile(inputFile);

        var entryPoint = typeof(Program).Assembly.EntryPoint!;
        var options = new string[]{
            "sops",
            "-f sops/sops.sec.yaml",
        };
        var returnObject = entryPoint.Invoke(null, new object[] { options });
        if(returnObject is Task returnTask)
        {
            await returnTask;
        }
        FileAssert.IsEqualToFile(expectedFile, consoleOutput.GetOuput());
    }
}
