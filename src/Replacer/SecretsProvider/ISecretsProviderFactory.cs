namespace replacer.SecretsProvider;

public enum SecretProviderTypes
{
    Sops,
}

public interface ISecretsProviderFactory
{
    ISecretsProvider GetProvider(SecretProviderTypes providerType);
}

public class SecretsProviderFactory : ISecretsProviderFactory
{
    private readonly Options options;

    public SecretsProviderFactory(Options options)
        => this.options = options;

    public ISecretsProvider GetProvider(SecretProviderTypes providerType)
        => providerType switch
        {
            SecretProviderTypes.Sops => new SopsSecretProvider(options),
            _ => throw new ArgumentOutOfRangeException(nameof(providerType), providerType, null),
        };
}
