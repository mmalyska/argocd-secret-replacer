namespace replacer.SecretsProvider;

using replacer.SecretsProvider.Sops;

public enum SecretProviderTypes
{
    sops,
}

public interface ISecretsProviderFactory
{
    ISecretsProvider GetProvider(Options options);
}

public class SecretsProviderFactory : ISecretsProviderFactory
{
    public ISecretsProvider GetProvider(Options options) => options switch
    {
        SopsOptions sopsOptions => new SopsSecretProvider(sopsOptions, new SopsProcessWrapper(sopsOptions)),
        _ => throw new ArgumentOutOfRangeException(nameof(options), options, null),
    };
}
