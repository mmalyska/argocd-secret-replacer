namespace ReplacerE2ETests;

using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;

public class TestFactory{
    public static async Task<TestcontainersContainer> PrepareSopsContainer(string sopsFile, IOutputConsumer output){
        var image = await new ImageFromDockerfileBuilder()
            .WithName("replacer-sops-test:latest")
            .WithDeleteIfExists(true)
            //.WithCleanUp(true)
            .WithDockerfile("sops/Dockerfile")
            .Build();

        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
            .WithImage(image)
            .WithCommand($"./sops-replacer -t sops --sops-file {sopsFile} < sops/inputFile.yaml")
            .WithOutputConsumer(output)
            .WithEnvironment("SOPS_AGE_KEY", "AGE-SECRET-KEY-1CZ0LJMC9PJQL64WRDRM77VW2D3MAP3TZ86J836UNNYHE5YKN9WJQ77L8UE")
            .Build();

        return testcontainersBuilder;
    }
}
