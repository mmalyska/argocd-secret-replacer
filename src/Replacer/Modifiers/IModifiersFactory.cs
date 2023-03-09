namespace Replacer.Modifiers;

using System.Reflection;

public interface IModifiersFactory
{
    IModifier? GetModifier(string name);
}

public class ModifiersFactory : IModifiersFactory
{
    private readonly IEnumerable<IModifier> modifiersAvailable;

    public ModifiersFactory()
        => modifiersAvailable = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(t => t.GetInterfaces().Contains(typeof(IModifier)))
            .Select(t => Activator.CreateInstance(t) as IModifier)
            .Where(m => m is not null)!;

    public IModifier? GetModifier(string name)
        => modifiersAvailable.SingleOrDefault(t => t.Key == name);
}
