namespace Replacer.Modifiers;

using System.Reflection;

public interface IModifiersFactory
{
    IModifier? GetModifier(string name);
}

public class ModifiersFactory : IModifiersFactory
{
    private readonly Dictionary<string, IModifier> modifiers;

    public ModifiersFactory()
        => modifiers = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Contains(typeof(IModifier)))
            .Select(t => Activator.CreateInstance(t) as IModifier)
            .Where(m => m is not null)
            .ToDictionary(m => m!.Key, m => m!);

    public IModifier? GetModifier(string name)
        => modifiers.GetValueOrDefault(name);
}
