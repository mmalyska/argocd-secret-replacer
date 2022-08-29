namespace ReplacerE2ETests.Utils;

using System;
using Xunit;

public class SopsEnvironmentFixture : IDisposable
{
    private const string sops_age_env = "SOPS_AGE_KEY_FILE";
    private const string sops_file_location = "sops/key.txt";
    private bool disposedValue;

    public SopsEnvironmentFixture()
        => Environment.SetEnvironmentVariable(sops_age_env, sops_file_location);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                Environment.SetEnvironmentVariable(sops_age_env, string.Empty);
            }

            disposedValue = true;
        }
    }
}

[CollectionDefinition("SopsEnv")]
public class SopsEnv : ICollectionFixture<SopsEnvironmentFixture>
{
}
