namespace ReplacerUnitTests.Sops;

using System.IO;
using System.Threading.Tasks;
using Replacer;
using Replacer.SecretsProvider;
using Replacer.SecretsProvider.Sops;
using Xunit;

public class SopsSecretProviderTests
{
    [Fact]
    public void WhenJsonFileShouldDeserializeJson()
    {
        var options = new SopsOptions { File = "something.json"};
        var wrapper = new StaticProcessWrapper("{\"key1\": \"val1\"}");
        var provider = new SopsSecretProvider(options, wrapper);
        var result = provider.GetSecret("key1");

        Assert.Equal("val1", result);
    }

    [Fact]
    public void WhenYamlFileShouldDeserializeYaml()
    {
        const string yamlFile = @"
        kind: Secret
        data:
            key1: val1";
        var options = new SopsOptions { File = "something.yaml"};
        var wrapper = new StaticProcessWrapper(yamlFile);
        var provider = new SopsSecretProvider(options, wrapper);
        var result = provider.GetSecret("key1");

        Assert.Equal("val1", result);
    }

    private sealed class StaticProcessWrapper : IProcessWrapper
    {
        private readonly MemoryStream memoryStream;
        private readonly StreamWriter writer;

        public StaticProcessWrapper(string output)
        {
            memoryStream = new MemoryStream();
            writer = new StreamWriter(memoryStream);
            writer.Write(output);
            writer.Flush();
            memoryStream.Position = 0;
            StandardOutput = new StreamReader(memoryStream);
        }

        public void Dispose()
        {
            memoryStream.Dispose();
            writer.Dispose();
            StandardOutput.Dispose();
        }

        public StreamReader StandardOutput { get; }
        public void Start() { }
        public Task WaitForExitAsync()
            => Task.CompletedTask;
    }
}
