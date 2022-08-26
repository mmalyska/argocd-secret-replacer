namespace Replacer.SecretsProvider.Sops;

using System.Text.Json;
using System.Text.Json.Serialization;

public class JsonDeserializer{
    public static Dictionary<string, string> GetSecretValues(string input){
        return JsonSerializer.Deserialize<Dictionary<string, string>>(input, SourceGenerationContext.Default.DictionaryStringString) ?? new Dictionary<string, string>();
    }
}

[JsonSerializable(typeof(Dictionary<string, string>))]
partial class SourceGenerationContext : JsonSerializerContext { }
