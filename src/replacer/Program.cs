using System.Text.RegularExpressions;
using CommandLine;
using Microsoft.Extensions.Logging;
using static CommandLine.Parser;
using replacer;

Console.WriteLine("Hello, World!");

using var loggerFactory = new LoggerFactory();

var parser = Default.ParseArguments(() => new Options(), args)
    .WithNotParsed(errors =>
    {
        var logger = loggerFactory.CreateLogger<Program>();
        LoggerMessages.LogErrorWrongParams(logger, string.Join(Environment.NewLine, errors.Select(error => error.ToString())));
        Environment.Exit(2);
    });

await parser.WithParsedAsync(options => RunOptions(options));

static async Task RunOptions(Options opts)
{
    var regexKey = new Regex(@"<[ \t]*(secret|sops):[^\r\n]+?>");
    while (await Console.In.ReadLineAsync() is { } line)
    {
        var replaced = regexKey.Replace(line, "[replaced]");
        await Console.Out.WriteAsync(replaced);
    }
}
