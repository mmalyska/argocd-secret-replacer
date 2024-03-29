namespace Replacer.SecretsProvider.Sops;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public static class YamlDeserializer
{
    public static Dictionary<string, string> GetSecretValues(string input)
    {
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();
        var obj = deserializer.Deserialize<SecretObject>(input);
        return obj.Data;
    }

    private sealed class SecretObject
    {
        public Dictionary<string, string> Data { get; set; } = new();
    }
}
