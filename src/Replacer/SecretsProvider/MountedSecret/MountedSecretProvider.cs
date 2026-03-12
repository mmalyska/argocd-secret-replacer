namespace Replacer.SecretsProvider.MountedSecret;

public sealed class MountedSecretProvider : ISecretsProvider
{
    private readonly Dictionary<string, string> secrets;

    public MountedSecretProvider(MountedSecretOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.MountPath))
            throw new ArgumentNullException(nameof(options));

        if (!Directory.Exists(options.MountPath))
            throw new DirectoryNotFoundException($"Mounted secret directory not found: {options.MountPath}");

        secrets = Directory.EnumerateFiles(options.MountPath)
            .ToDictionary(
                f => Path.GetFileName(f)!,
                f => File.ReadAllText(f).TrimEnd('\n', '\r', ' '));
    }

    public string GetSecret(string key)
        => secrets.TryGetValue(key, out var value) ? value : string.Empty;
}
