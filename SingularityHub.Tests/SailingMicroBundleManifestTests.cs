using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SailingMicroBundleManifestTests
{
    [Fact(DisplayName = "Sailing micro-bundle begins with a player-pilotable sailboat")]
    public void Sailing_StartsWithPersonalSailboat()
    {
        var bundle = SailingMicroBundleManifest.CreateDefault();
        var sailboat = Assert.Single(bundle.Vessels, x => x.Id == "sailboat.personal");

        Assert.True(sailboat.PlayerCanPilot);
        Assert.Equal(SailingVesselKind.Sailboat, sailboat.Kind);
    }

    [Fact(DisplayName = "Sailing micro-bundle grows from solo sailing to crew and fleet play")]
    public void Sailing_ContainsProgressionAndCrewSystems()
    {
        var bundle = SailingMicroBundleManifest.CreateDefault();

        Assert.Contains(bundle.GameplaySystems, x => x.Id == "wind");
        Assert.Contains(bundle.GameplaySystems, x => x.Id == "crew");
        Assert.Contains(bundle.GameplaySystems, x => x.Id == "naval-combat");
        Assert.Contains(bundle.Progression, x => x.Id == "age-of-sail");
        Assert.Contains(bundle.Progression, x => x.Id == "modern");
    }
}
