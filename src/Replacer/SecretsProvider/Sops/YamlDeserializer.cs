namespace Replacer.SecretsProvider.Sops;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class YamlDeserializer{
    public static Dictionary<string, string> GetSecretValues(string input){
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();
        var obj = deserializer.Deserialize<SecretObject>(input);
        return obj.Data;
    }

    class SecretObject{
        public Dictionary<string, string> Data { get; set; } = new Dictionary<string, string>();
    }
}
