namespace Replacer.SecretsProvider.Sops;

public sealed class SopsSecretProvider : ISecretsProvider, IDisposable
{
    private readonly IProcessWrapper process;
    private readonly string? sopsFile;
    private bool disposedValue;
    private Dictionary<string, string>? secretValues;

    public SopsSecretProvider(SopsOptions options, IProcessWrapper processWrapper)
    {
        sopsFile = options.File ?? throw new ArgumentNullException(nameof(options));
        process = processWrapper ?? throw new ArgumentNullException(nameof(processWrapper));
        DecodeSecretsAsync().Wait();
    }

    public void Dispose()
        => Dispose(true);

    public string GetSecret(string key)
    {
        secretValues!.TryGetValue(key, out var value);
        return value ?? string.Empty;
    }

    private async Task DecodeSecretsAsync()
    {
        process.Start();
        var serializedData = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync();

        secretValues = Path.GetExtension(sopsFile) switch
        {
            ".json" => JsonDeserializer.GetSecretValues(serializedData),
            ".yml" or ".yaml" => YamlDeserializer.GetSecretValues(serializedData),
            _ => new Dictionary<string, string>(),
        };
    }

    private void Dispose(bool disposing)
    {
        if (disposedValue)
        {
            return;
        }

        if (disposing)
        {
            process.Dispose();
        }

        disposedValue = true;
    }
}
