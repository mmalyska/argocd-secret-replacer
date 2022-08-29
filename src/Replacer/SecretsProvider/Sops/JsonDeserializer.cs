namespace Replacer.SecretsProvider.Sops;

using System.Text.Json;
using System.Text.Json.Serialization;

public static class JsonDeserializer
{
    public static Dictionary<string, string> GetSecretValues(string input)
        => JsonSerializer.Deserialize(input, SourceGenerationContext.Default.DictionaryStringString) ?? new Dictionary<string, string>();
}

[JsonSerializable(typeof(Dictionary<string, string>))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}
