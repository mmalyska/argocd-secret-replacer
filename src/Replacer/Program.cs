using CommandLine;
using Microsoft.Extensions.Logging;
using Replacer;
using Replacer.Modifiers;
using Replacer.SecretsProvider;
using Replacer.Substitution;
using static CommandLine.Parser;

var parser = Default.ParseArguments(args, typeof(SopsOptions))
    .WithNotParsed(ParseErrors);

parser.WithParsed(RunOptions);

static void RunOptions(object opts)
{
    ISecretsProviderFactory providerFactory = new SecretsProviderFactory();
    IModifiersFactory modifiersFactory = new ModifiersFactory();
    var provider = providerFactory.GetProvider(opts);
    ISecretReplacer replacer = new SecretReplacer(provider, modifiersFactory);

    var data = Console.In.ReadToEnd();
    Console.Out.Write(replacer.Replace(data));
}

static void ParseErrors(IEnumerable<Error> errors)
{
    using var loggerFactory = new LoggerFactory();
    var logger = loggerFactory.CreateLogger<Program>();
    LoggerMessages.LogErrorWrongParams(logger, string.Join(Environment.NewLine, errors.Select(error => error.ToString())));
    Environment.Exit(2);
}
