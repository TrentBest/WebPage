using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SpatialGardenTests
{
    [Fact(DisplayName = "Incremental Unit Test 63 — first-visit garden teaches the threshold before the Workshop")]
    public void GardenProvidesHumanScaleThreshold()
    {
        var garden = SpatialGardenManifest.CreateDefault();

        Assert.Equal("welcome-garden", garden.Id);
        Assert.Equal("THE GARDEN", garden.Name);
        Assert.Contains(garden.Gates, x => x.Id == "hangout" && x.Side == SpatialGardenGateSide.Left);
        Assert.Contains(garden.Gates, x => x.Id == "exit" && x.Side == SpatialGardenGateSide.Center);
        Assert.Contains(garden.Gates, x => x.Id == "singularity-station" && x.Side == SpatialGardenGateSide.Right);
        Assert.Equal("SOCIAL GAZEBO", garden.Gazebo.Name);
        Assert.Equal("SETTING SHED", garden.SettingsShed.Name);
    }

    [Fact(DisplayName = "Incremental Unit Test 64 — garden destinations are explicit and navigable")]
    public void GardenDestinationsAreExplicit()
    {
        var garden = SpatialGardenManifest.CreateDefault();

        Assert.Equal("hangout", garden.FindGate("hangout")!.DestinationId);
        Assert.Equal("singularity-station", garden.FindGate("singularity-station")!.DestinationId);
        Assert.Equal("exit", garden.FindGate("exit")!.DestinationId);
    }
}
