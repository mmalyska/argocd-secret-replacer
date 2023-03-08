namespace Replacer.Substitution;

using System.Text;
using System.Text.RegularExpressions;
using Modifiers;
using SecretsProvider;

public interface ISecretReplacer
{
    public string Replace(string line);
}

public partial class SecretReplacer : ISecretReplacer
{
    private readonly ISecretsProvider secretsProvider;
    private readonly IModifiersFactory modifiersFactory;

    public SecretReplacer(ISecretsProvider secretsProvider, IModifiersFactory modifiersFactory)
    {
        this.secretsProvider = secretsProvider ?? throw new ArgumentNullException(nameof(secretsProvider));
        this.modifiersFactory = modifiersFactory ?? throw new ArgumentNullException(nameof(modifiersFactory));
    }

    public string Replace(string line)
        => ReplaceKey(ReplaceBase64(line));

    private string ReplaceBase64(string line)
        => RegexBase64Generated().Replace(line, Base64Evaluator);

    private string ReplaceKey(string line)
        => RegexKeyGenerated().Replace(line, Evaluator);

    private string Evaluator(Match match)
    {
        var path = match.Groups["path"].Value;
        var modifiers = match.Groups["modifiers"].Value.Split('|', StringSplitOptions.RemoveEmptyEntries);
        var secret = secretsProvider.GetSecret(path);
        foreach (var modifierName in modifiers)
        {
            var modifier = modifiersFactory.GetModifier(modifierName);
            secret = modifier?.Apply(secret) ?? secret;
        }
        return match.Success
            ? secret
            : match.Value;
    }

    private string Base64Evaluator(Group match)
    {
        if (!match.Success)
        {
            return match.Value;
        }

        var (decodedString, error) = FromBase64(match.Value);
        if (error)
        {
            return match.Value;
        }

        var replaceResult = ReplaceKey(decodedString);
        return replaceResult == decodedString
            ? match.Value
            : ToBase64(replaceResult);
    }

    private static (string value, bool error) FromBase64(string data)
    {
        try
        {
            var bytes = Convert.FromBase64String(data);
            var value = Encoding.UTF8.GetString(bytes);
            return (value, false);
        }
        catch (FormatException)
        {
            return (data, true);
        }
    }

    private static string ToBase64(string data)
    {
        var bytes = Encoding.UTF8.GetBytes(data);
        return Convert.ToBase64String(bytes);
    }

    [GeneratedRegex("<[ \\t]*(?<store>secret|sops):(?<path>[^\\r\\n\\|>]*)\\|?(?<modifiers>[^\\r\\n>]*)>", RegexOptions.Compiled | RegexOptions.Singleline)]
    private static partial Regex RegexKeyGenerated();
    [GeneratedRegex("[A-Za-z0-9\\+\\/\\=]{10,}", RegexOptions.Compiled | RegexOptions.Singleline)]
    private static partial Regex RegexBase64Generated();
}
