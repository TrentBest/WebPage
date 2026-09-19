using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SingularityRolePlayingSystemTests
{
    [Fact]
    public void DefaultSystem_ContainsPhysicsAndMaterialFoundations()
    {
        var system = SingularityRolePlayingSystem.CreateDefault();

        Assert.Equal("srps", system.Id);
        Assert.Contains(system.Attributes, x => x.Id == "mass");
        Assert.Contains(system.Materials, x => x.Id == "bronze");
        Assert.Contains(system.Materials, x => x.Id == "iron");
        Assert.Contains(system.PhysicsRuleSets, x => x.Id == "material-impact");
    }

    [Fact]
    public void PhysicsBake_RepresentsMillionSampleRuntimeLut()
    {
        var bake = SrpsPhysicsBakeManifest.BronzeVsIronImpact;

        Assert.True(bake.Lut.IsBaked);
        Assert.Equal(1_000_000, bake.Lut.SampleCount);
        Assert.Contains("material-pair", bake.Lut.CoordinateSchema);
    }
}
