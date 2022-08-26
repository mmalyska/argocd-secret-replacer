namespace Replacer;

using CommandLine;
using Replacer.SecretsProvider;

public abstract class Options {
    public abstract SecretProviderTypes ProviderType { get; }
}

[Verb("sops", HelpText = "Manage sops options")]
public class SopsOptions : Options
{
    [Option('f', "file", Required = false, HelpText = "Sops file to open")]
    public string? File { get; set; } = string.Empty;

    public override SecretProviderTypes ProviderType => SecretProviderTypes.sops;
}
