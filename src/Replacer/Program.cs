using CommandLine;
using Microsoft.Extensions.Logging;
using static CommandLine.Parser;
using Replacer;
using Replacer.SecretsProvider;
using Replacer.Substitution;

if (!Console.IsInputRedirected)
{
    return;
}

using var loggerFactory = new LoggerFactory();

var parser = Default.ParseArguments<SopsOptions>(args)
    .WithNotParsed(errors =>
    {
        var logger = loggerFactory.CreateLogger<Program>();
        LoggerMessages.LogErrorWrongParams(logger, string.Join(Environment.NewLine, errors.Select(error => error.ToString())));
        Environment.Exit(2);
    });

await parser.WithParsedAsync(RunOptions);

static async Task RunOptions(Options opts)
{
    var providerFactory = new SecretsProviderFactory();
    var provider = providerFactory.GetProvider(opts);
    ISecretReplacer replacer = new SecretReplacer(provider);

    while (await Console.In.ReadLineAsync() is { } line)
    {
        if (Console.In.Peek() == -1)
            await Console.Out.WriteAsync(replacer.Replace(line));
        else
            await Console.Out.WriteLineAsync(replacer.Replace(line));
    }
}
