namespace replacer.SecretsProvider;

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

public class SopsSecretProvider : ISecretsProvider
{
    private readonly string? sopsFile;
    private readonly IEnumerable<string> sopsOptions;
    private SecretObject? secretValues;

    public SopsSecretProvider(Options options)
    {
        sopsFile = options.SopsFile ?? throw new InvalidOperationException();
        sopsOptions = options.SopsOptions ?? throw new InvalidOperationException();
    }

    public async Task<string> GetSecretAsync(string key)
    {
        if (secretValues is null)
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "sops",
                    Arguments = sopsOptions + " -d " + sopsFile,
                    RedirectStandardOutput = true
                },
            };

            process.Start();
            var serializedData = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();
            secretValues = JsonSerializer.Deserialize<SecretObject>(serializedData, SourceGenerationContext.Default.SecretObject) ?? new SecretObject();
        }
        secretValues.TryGetValue(key, out var value);
        return value ?? string.Empty;
    }
}

public class SecretObject: Dictionary<string, string>{}

[JsonSerializable(typeof(SecretObject))]
internal partial class SourceGenerationContext : JsonSerializerContext { }
