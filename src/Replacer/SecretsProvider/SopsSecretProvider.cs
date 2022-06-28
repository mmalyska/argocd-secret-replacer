namespace replacer.SecretsProvider;

public class SopsSecretProvider : ISecretsProvider
{
    public string GetSecret(string key)
        => throw new NotImplementedException();
}
