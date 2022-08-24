namespace replacer.SecretsProvider.Sops;

using System.Diagnostics;

public class SopsProcessWrapper : SystemProcess
{
    public SopsProcessWrapper(Options options): base()
    {
        var executable = Environment.GetEnvironmentVariable("ARGOCD_ENV_SOPS_EXE") ?? "sops";

        var processStartInfo = new ProcessStartInfo
        {
            FileName = executable,
            Arguments = "-d " + options.SopsFile,
            RedirectStandardOutput = true
        };

        process.StartInfo = processStartInfo;
    }
}
