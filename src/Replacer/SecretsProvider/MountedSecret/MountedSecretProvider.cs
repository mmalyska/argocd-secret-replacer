namespace Replacer.SecretsProvider.MountedSecret;

public sealed class MountedSecretProvider : ISecretsProvider
{
    private readonly string mountPath;

    public MountedSecretProvider(MountedSecretOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.MountPath))
            throw new ArgumentNullException(nameof(options));

        mountPath = options.MountPath;

        if (!Directory.Exists(mountPath))
            throw new DirectoryNotFoundException($"Mounted secret directory not found: {mountPath}");
    }

    public string GetSecret(string key)
    {
        var filePath = Path.Combine(mountPath, key);

        if (!File.Exists(filePath))
            return string.Empty;

        return File.ReadAllText(filePath).TrimEnd('\n', '\r', ' ');
    }
}
