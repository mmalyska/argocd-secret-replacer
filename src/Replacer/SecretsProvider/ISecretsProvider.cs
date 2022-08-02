namespace replacer.SecretsProvider;

public interface ISecretsProvider
{
    Task<string> GetSecretAsync(string key);
}
