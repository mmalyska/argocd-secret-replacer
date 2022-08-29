namespace ReplacerE2ETests.Utils;

using System;
using Xunit;

public class SopsEnvironmentFixture : IDisposable
{
    private const string SopsAgeEnv = "SOPS_AGE_KEY_FILE";
    private const string SopsFileLocation = "sops/key.txt";
    private bool disposedValue;

    public SopsEnvironmentFixture()
        => Environment.SetEnvironmentVariable(SopsAgeEnv, SopsFileLocation);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposedValue)
        {
            return;
        }

        if (disposing)
        {
            Environment.SetEnvironmentVariable(SopsAgeEnv, string.Empty);
        }

        disposedValue = true;
    }
}

[CollectionDefinition("SopsEnv")]
public class SopsEnv : ICollectionFixture<SopsEnvironmentFixture>
{
}
