namespace Replacer.SecretsProvider;

using System.Diagnostics;

public interface IProcessWrapper
{
    StreamReader StandardOutput { get; }
    void Start();
    Task WaitForExitAsync();
}

public abstract class SystemProcess : IProcessWrapper, IDisposable
{
    private readonly Process process;
    private bool disposedValue;

    protected SystemProcess()
        => process = new Process();

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public StreamReader StandardOutput => process.StandardOutput;
    public void Start() => process.Start();
    public Task WaitForExitAsync() => process.WaitForExitAsync();

    protected void SetStartInfo(ProcessStartInfo startInfo)
        => process.StartInfo = startInfo;

    private void Dispose(bool disposing)
    {
        if (disposedValue)
        {
            return;
        }

        if (disposing)
        {
            process.Dispose();
        }

        disposedValue = true;
    }
}
