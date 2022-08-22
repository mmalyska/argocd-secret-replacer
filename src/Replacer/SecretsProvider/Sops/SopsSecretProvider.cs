namespace replacer.SecretsProvider.Sops;

using System.Collections.Generic;

public class SopsSecretProvider : ISecretsProvider, IDisposable
{
    private readonly string? sopsFile;
    private Dictionary<string, string>? secretValues;
    private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1);
    private bool disposedValue;
    private readonly IProcessWrapper process;

    public SopsSecretProvider(Options options, IProcessWrapper processWrapper)
    {
        sopsFile = options.SopsFile ?? throw new ArgumentNullException(nameof(options));
        process = processWrapper ?? throw  new ArgumentNullException(nameof(processWrapper));
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

    private async Task DecodeSecretsAsync(){
        if (secretValues is not null)
        {
            return;
        }

        process.Start();
        var serializedData = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync();

        secretValues = Path.GetExtension(sopsFile) switch
        {
            "json" => JsonDeserializer.GetSecretValues(serializedData),
            "yml" or "yaml" => YamlDeserializer.GetSecretValues(serializedData),
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

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
