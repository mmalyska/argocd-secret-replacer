namespace replacer.SecretsProvider.Sops;

using System.Diagnostics;

public class SopsProcessWrapper : SystemProcess
{
    public SopsProcessWrapper(Options options): base()
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "sops",
            Arguments = "-d " + options.SopsFile,
            RedirectStandardOutput = true
        };

        process.StartInfo = processStartInfo;
    }
}
