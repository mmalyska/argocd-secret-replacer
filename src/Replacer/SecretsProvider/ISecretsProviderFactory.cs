namespace Replacer.SecretsProvider;

using Sops;

public interface ISecretsProviderFactory
{
    ISecretsProvider GetProvider(object options);
}

public class SecretsProviderFactory : ISecretsProviderFactory
{
    public ISecretsProvider GetProvider(object options) => options switch
    {
        SopsOptions sopsOptions => new SopsSecretProvider(sopsOptions, new SopsProcessWrapper(sopsOptions)),
        _ => throw new ArgumentOutOfRangeException(nameof(options), options, null),
    };
}
