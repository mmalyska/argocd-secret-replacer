namespace Replacer.SecretsProvider.Sops;

public class SopsSecretProvider : ISecretsProvider, IDisposable
{
    private readonly IProcessWrapper process;
    private readonly string? sopsFile;
    private bool disposedValue;
    private Dictionary<string, string>? secretValues;
    private readonly SemaphoreSlim semaphoreSlim = new(1);

    public SopsSecretProvider(SopsOptions options, IProcessWrapper processWrapper)
    {
        sopsFile = options.File ?? throw new ArgumentNullException(nameof(options));
        process = processWrapper ?? throw new ArgumentNullException(nameof(processWrapper));
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async Task<string> GetSecretAsync(string key)
    {
        if (secretValues is null)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                await DecodeSecretsAsync();
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        secretValues!.TryGetValue(key, out var value);
        return value ?? string.Empty;
    }

    private async Task DecodeSecretsAsync()
    {
        if (secretValues is not null)
        {
            return;
        }

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

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                semaphoreSlim.Dispose();
            }

            disposedValue = true;
        }
    }
}
