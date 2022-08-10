namespace replacer.SecretsProvider.Sops;

using System.Collections.Generic;
using System.Diagnostics;

public class SopsSecretProvider : ISecretsProvider
{
    private readonly string? sopsFile;
    private Dictionary<string, string>? secretValues;

    public SopsSecretProvider(Options options)
    {
        sopsFile = options.SopsFile ?? throw new ArgumentNullException();
    }

    public async Task<string> GetSecretAsync(string key)
    {
        if (secretValues is null)
        {
            await DecodeSecretsAsync();
        }
        secretValues!.TryGetValue(key, out var value);
        return value ?? string.Empty;
    }

    private async Task DecodeSecretsAsync(){
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "sops",
                Arguments = "-d " + sopsFile,
                RedirectStandardOutput = true
            },
        };

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
}
