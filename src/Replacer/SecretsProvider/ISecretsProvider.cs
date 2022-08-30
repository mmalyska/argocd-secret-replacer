namespace Replacer.SecretsProvider;

public interface ISecretsProvider
{
    string GetSecret(string key);
}
