namespace replacer.SecretsProvider;

using System.Diagnostics;

public class SopsSecretProvider : ISecretsProvider
{
    private readonly string sopsOptions;
    private Dictionary<string,string>? secretValues;

    public SopsSecretProvider(Options options)
        => sopsOptions = options.SopsOptions ?? throw new InvalidOperationException();

    public async Task<string> GetSecretAsync(string key)
    {
        if (secretValues is null)
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "sops",
                    Arguments = sopsOptions,
                },
            };

            process.Start();
            await process.WaitForExitAsync();
            var decodedFile = await process.StandardOutput.ReadToEndAsync();
            secretValues = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string,string>>(decodedFile) ?? new Dictionary<string, string>();
        }
        secretValues.TryGetValue(key, out var value);
        return value ?? string.Empty;
    }
}
