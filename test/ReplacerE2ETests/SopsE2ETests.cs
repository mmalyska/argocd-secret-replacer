namespace ReplacerE2ETests;

using System;
using System.Threading.Tasks;
using Xunit;

public class SopsE2ETests{

    public SopsE2ETests()
    {
        
    }

    [SkippableFact]
    public async Task TestJsonFile(){

        Skip.IfNot(System.OperatingSystem.IsLinux(), "Test designed for Linux");

        var entryPoint = typeof(Program).Assembly.EntryPoint!;
        var returnObject = entryPoint.Invoke(null, new object[] { Array.Empty<string>() });
        if(returnObject is Task returnTask){
            await returnTask;
        }
    }
}
