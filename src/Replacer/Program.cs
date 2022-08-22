using CommandLine;
using Microsoft.Extensions.Logging;
using static CommandLine.Parser;
using replacer;
using replacer.SecretsProvider;
using replacer.Substitution;

if (!Console.IsInputRedirected)
{
    return;
}

using var loggerFactory = new LoggerFactory();

var parser = Default.ParseArguments(() => new Options(), args)
    .WithNotParsed(errors =>
    {
        var logger = loggerFactory.CreateLogger<Program>();
        LoggerMessages.LogErrorWrongParams(logger, string.Join(Environment.NewLine, errors.Select(error => error.ToString())));
        Environment.Exit(2);
    });

await parser.WithParsedAsync(RunOptions);

static async Task RunOptions(Options opts)
{
    var providerFactory = new SecretsProviderFactory(opts);
    var provider = providerFactory.GetProvider(opts.SecretType);
    IReplacer replacer = new Replacer(provider);

    while (await Console.In.ReadLineAsync() is { } line)
    {
        await Console.Out.WriteLineAsync(replacer.Replace(line));
    }
}
