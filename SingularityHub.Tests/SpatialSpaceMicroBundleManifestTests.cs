using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SpatialSpaceMicroBundleManifestTests
{
    [Fact]
    public void SpaceBundle_SeparatesPhysicsAndWeaponsSystems()
    {
        var bundle = SpatialSpaceMicroBundleManifest.CreateDefault();

        Assert.Contains(bundle.PhysicsSystems, x => x.Id == "orbital");
        Assert.Contains(bundle.PhysicsSystems, x => x.Id == "vacuum");
        Assert.Contains(bundle.WeaponsSystems, x => x.Id == "kinetic");
        Assert.Contains(bundle.WeaponsSystems, x => x.Id == "directed-energy");
    }
}
