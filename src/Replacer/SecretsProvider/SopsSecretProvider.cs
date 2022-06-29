namespace replacer.SecretsProvider;

using System.Diagnostics;

public class SopsSecretProvider : ISecretsProvider
{
    private readonly string sopsOptions;

    public SopsSecretProvider(Options options)
        => sopsOptions = options.SopsOptions ?? throw new InvalidOperationException();

    public async Task<string> GetSecretAsync(string key)
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
        return await process.StandardOutput.ReadToEndAsync();
    }
}
