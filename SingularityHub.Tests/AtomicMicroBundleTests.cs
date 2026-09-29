using TheSingularityWorkshop.Infrastructure.Atomic;
using TheSingularityWorkshop.Infrastructure.FsmForge;
using Xunit;

namespace SingularityHub.Tests;

public sealed class AtomicMicroBundleTests
{
    [Fact(DisplayName = "Atomic core stays small while optional thermal data remains separate")]
    public void CoreAndThermalDataAreSeparate()
    {
        var core = new AtomicCoreBundle(new AtomicElement(26, "Fe", "Iron", 55.845));
        var thermal = new AtomicThermalBundle(1811, 3134);
        Assert.Equal(AtomicBundleIds.Core, core.Id);
        Assert.Equal("Fe", core.Element.Symbol);
        Assert.Equal(AtomicBundleIds.Thermal, thermal.Id);
        Assert.Equal(1811, thermal.MeltingPointKelvin);
        Assert.DoesNotContain(core.Dependencies, dependency => dependency.BundleId == AtomicBundleIds.Thermal);
    }

    [Fact(DisplayName = "Material physics explicitly pulls the thermal capability")]
    public void MaterialPhysicsDeclaresOptionalDependencies()
    {
        var physics = new AtomicMaterialPhysicsBundle();
        Assert.Contains(physics.Dependencies, dependency => dependency.BundleId == AtomicBundleIds.Core);
        Assert.Contains(physics.Dependencies, dependency => dependency.BundleId == AtomicBundleIds.Thermal);
    }

    [Fact(DisplayName = "Forge logic context can carry only the MicroBundles the experience requested")]
    public void LogicContextCarriesSelectedBundles()
    {
        var context = new FsmForgeLogicContext();
        var core = new AtomicCoreBundle(new AtomicElement(26, "Fe", "Iron", 55.845));
        context.AttachBundle(core);
        Assert.True(context.TryGetBundle<AtomicCoreBundle>(AtomicBundleIds.Core, out var resolved));
        Assert.Same(core, resolved);
        Assert.False(context.TryGetBundle<AtomicThermalBundle>(AtomicBundleIds.Thermal, out _));
    }
}
