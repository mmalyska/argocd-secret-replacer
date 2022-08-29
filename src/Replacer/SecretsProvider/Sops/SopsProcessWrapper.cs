namespace Replacer.SecretsProvider.Sops;

using System.Diagnostics;

public class SopsProcessWrapper : SystemProcess
{
    public SopsProcessWrapper(SopsOptions options)
    {
        var executable = Environment.GetEnvironmentVariable("ARGOCD_ENV_SOPS_EXE") ?? "sops";

        var processStartInfo = new ProcessStartInfo { FileName = executable, Arguments = "-d " + options.File, RedirectStandardOutput = true };

        SetStartInfo(processStartInfo);
    }
}
