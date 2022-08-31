namespace Replacer.Modifiers;

public interface IModifier
{
    string Key { get; }
    string Apply(string data);
}
