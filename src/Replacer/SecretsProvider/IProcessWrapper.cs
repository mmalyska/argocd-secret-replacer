namespace replacer.SecretsProvider;

using System.Diagnostics;

public interface IProcessWrapper
{
    StreamReader StandardOutput { get; }
    void Start();
    Task WaitForExitAsync();
}

public abstract class SystemProcess: IProcessWrapper, IDisposable
{
    protected readonly Process process;
    private bool disposedValue;

    protected SystemProcess()
    {
        process = new Process();
    }

    public StreamReader StandardOutput => process.StandardOutput;
    public void Start() => process.Start();
    public Task WaitForExitAsync() => process.WaitForExitAsync();

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                process.Dispose();
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
