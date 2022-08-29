namespace ReplacerE2ETests.Utils;

using System;
using Xunit;

public class SopsEnvironmentFixture : IDisposable
{
    private const string sops_age_env = "SOPS_AGE_KEY_FILE";
    private const string sops_file_location = "sops/key.txt";
    private bool disposedValue;

    public SopsEnvironmentFixture()
    {
        Environment.SetEnvironmentVariable(sops_age_env, sops_file_location);
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

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

[CollectionDefinition("SopsEnv")]
public class SopsEnv : ICollectionFixture<SopsEnvironmentFixture>{}
