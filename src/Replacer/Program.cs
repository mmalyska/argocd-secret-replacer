using CommandLine;
using Microsoft.Extensions.Logging;
using Replacer;
using Replacer.SecretsProvider;
using Replacer.Substitution;
using static CommandLine.Parser;

if (!Console.IsInputRedirected)
{
    return;
}

using var loggerFactory = new LoggerFactory();

var parser = Default.ParseArguments(args, typeof(SopsOptions))
    .WithNotParsed(errors =>
    {
        var logger = loggerFactory.CreateLogger<Program>();
        LoggerMessages.LogErrorWrongParams(logger, string.Join(Environment.NewLine, errors.Select(error => error.ToString())));
        Environment.Exit(2);
    });

parser.WithParsed(RunOptions);

static void RunOptions(object opts)
{
    var providerFactory = new SecretsProviderFactory();
    var provider = providerFactory.GetProvider(opts);
    ISecretReplacer replacer = new SecretReplacer(provider);

    var data = Console.In.ReadToEnd();
    Console.Out.Write(replacer.Replace(data));
}
