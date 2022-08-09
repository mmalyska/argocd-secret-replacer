namespace replacer;

using CommandLine;
using SecretsProvider;

public class Options
{
    [Option('t', "type", Required = true, HelpText = "Type of secret to process")]
    public SecretProviderTypes SecretType { get; set; }

    [Option("sops-file", Required = false, HelpText = "Sops file to open")]
    public string? SopsFile { get; set; } = string.Empty;

    [Option("sops-options", Required = false, HelpText = "Sops options to use")]
    public IEnumerable<string> SopsOptions { get; set; } = new List<string>();
}
