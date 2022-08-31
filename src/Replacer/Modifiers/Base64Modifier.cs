namespace Replacer.Modifiers;

using System.Text;

public class Base64Modifier : IModifier
{
    public string Key => "base64";

    public string Apply(string data)
        => ToBase64(data);

    private static string ToBase64(string data)
    {
        var bytes = Encoding.UTF8.GetBytes(data);
        return Convert.ToBase64String(bytes);
    }
}
