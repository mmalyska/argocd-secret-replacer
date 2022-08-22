namespace ReplacerE2ETests;

using System.IO;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Xunit;

public class SopsE2ETests{

    public SopsE2ETests()
    {
        
    }

    [SkippableFact]
    public async Task TestJsonFile(){

        Skip.IfNot(System.OperatingSystem.IsLinux(), "Test designed for Linux");

        
    }
}
