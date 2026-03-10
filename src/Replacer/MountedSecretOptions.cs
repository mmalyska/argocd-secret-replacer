namespace Replacer;

using CommandLine;

[Verb("secret", HelpText = "Read secrets from a Kubernetes Secret mounted as a directory")]
public class MountedSecretOptions
{
    [Option('m', "mount", Required = true, HelpText = "Path to the mounted Kubernetes Secret directory")]
    public string MountPath { get; set; } = string.Empty;
}
