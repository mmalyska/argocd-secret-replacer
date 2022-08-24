namespace replacer.SecretsProvider;

using replacer.SecretsProvider.Sops;

public enum SecretProviderTypes
{
    sops,
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
            SecretProviderTypes.sops => new SopsSecretProvider(options, new SopsProcessWrapper(options)),
            _ => throw new ArgumentOutOfRangeException(nameof(providerType), providerType, null),
        };
}
