namespace replacer;

using CommandLine;
using SecretsProvider;

public class Options
{
    [Option('t', "type", Required = true, HelpText = "Type of secret to process")]
    public SecretProviderTypes? SecretType { get; set; }

    [Option("sops", Required = false, HelpText = "Sops options to pass")]
    public string? SopsOptions { get; set; }
}
