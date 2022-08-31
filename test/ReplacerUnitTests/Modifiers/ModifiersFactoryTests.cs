namespace ReplacerUnitTests.Modifiers;

using Replacer.Modifiers;
using Xunit;

public class ModifiersFactoryTests
{
    [Fact]
    public void FindModifierShouldReturn()
    {
        var modifierFactory = new ModifiersFactory();
        var modifier = modifierFactory.GetModifier("test");

        Assert.NotNull(modifier);
        Assert.IsType<TestModifier>(modifier);
    }
}

public class TestModifier : IModifier
{
    public string Key => "test";
    public string Apply(string data)
        => data;
}
