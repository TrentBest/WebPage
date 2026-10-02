using TheSingularityWorkshop.Workshop.Chemistry;
using Xunit;

namespace SingularityHub.Tests;

public sealed class FictionalElementRegistryTests
{
    [Fact]
    public void Registry_Accepts_Fictional_Elements_And_Preserves_Their_Identity()
    {
        var registry = new FictionalElementRegistry();
        var element = new AtomBuilder("Dilithium", 0, "Dl")
            .WithAtomicWeight(0)
            .WithOrigin(ElementOrigin.Fictional)
            .WithCategory("Fictional")
            .Build();

        Assert.True(registry.Register(element));
        Assert.Equal(1, registry.Count);
        Assert.Same(element, registry.GetBySymbol("Dl"));
        Assert.True(registry.GetBySymbol("Dl").IsFictional);
    }

    [Fact]
    public void Registry_Rejects_Canonical_Elements()
    {
        var registry = new FictionalElementRegistry();

        var exception = Assert.Throws<System.ArgumentException>(
            () => registry.Register(ElementalCatalog.Core["Fe"]));

        Assert.Contains("Only fictional elements", exception.Message);
    }

    [Fact]
    public void Registry_Rejects_Symbol_Collision_With_Canonical_Element()
    {
        var registry = new FictionalElementRegistry();
        var fictionalIron = new AtomBuilder("Fictional Iron", 0, "Fe")
            .WithAtomicWeight(0)
            .WithOrigin(ElementOrigin.Fictional)
            .Build();

        var exception = Assert.Throws<System.ArgumentException>(
            () => registry.Register(fictionalIron));

        Assert.Contains("already assigned", exception.Message);
    }

    [Fact]
    public void Registry_Allows_Multiple_Fictional_Elements_From_A_Bundle()
    {
        var registry = new FictionalElementRegistry();

        var elements = new[]
        {
            new AtomBuilder("Dilithium", 0, "Dl")
                .WithOrigin(ElementOrigin.Fictional)
                .Build(),
            new AtomBuilder("Unobtanium-X", 0, "Ux")
                .WithOrigin(ElementOrigin.Fictional)
                .Build()
        };

        Assert.True(registry.RegisterRange(elements));
        Assert.Equal(2, registry.Count);
    }
}
