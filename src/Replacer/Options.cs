namespace Replacer;

using CommandLine;

[Verb("sops", HelpText = "Manage sops options")]
public class SopsOptions
{
    [Option('f', "file", Required = false, HelpText = "Sops file to open")]
    public string? File { get; set; } = string.Empty;
}
