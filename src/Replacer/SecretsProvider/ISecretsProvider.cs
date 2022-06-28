namespace replacer.SecretsProvider;

public interface ISecretsProvider
{
    string GetSecret(string key);
}
