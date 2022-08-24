namespace replacer.Substitution;

using System.Text.RegularExpressions;
using SecretsProvider;

public interface IReplacer
{
    public string Replace(string line);
}

public class Replacer : IReplacer
{
    private const string Pattern = @"<[ \t]*(?<store>secret|sops):(?<path>[^\r\n\|]*)\|?(?<modifiers>[^\r\n]*)>";
    private readonly Regex regexKey = new(Pattern, RegexOptions.Singleline | RegexOptions.Compiled);
    private readonly ISecretsProvider secretsProvider;

    public Replacer(ISecretsProvider secretsProvider)
    {
        this.secretsProvider = secretsProvider ?? throw new ArgumentNullException(nameof(secretsProvider));
    }

    public string Replace(string line)
        => regexKey.Replace(line, match => Evaluator(match, secretsProvider));

    private static string Evaluator(Match match, ISecretsProvider secretsProvider)
    {
        var path = match.Groups["path"].Value;
        var modifiers = match.Groups["modifiers"].Value.Split('|');
        var secret = secretsProvider.GetSecretAsync(path).Result;
        return string.IsNullOrEmpty(secret) ? match.Value : secret;
    }
}
