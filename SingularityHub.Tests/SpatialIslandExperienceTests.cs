using System.Linq;
using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>Heartbeat tests proving Singularity Island is a composed Experience surface.</summary>
public sealed class SpatialIslandExperienceTests
{
    [Fact]
    public void SingularityIsland_Contains_Discoverable_Experiences()
    {
        var island = SpatialIslandExperience.SingularityIsland;

        Assert.Equal("singularity-island", island.Id);
        Assert.Contains(island.Attractions, x => x.Id == "maze");
        Assert.Contains(island.Attractions, x => x.Id == "recipe-garden");
        Assert.Contains(island.Attractions, x => x.Id == "creator-archive");
    }

    [Fact]
    public void GrandCentral_Contains_SingularityIsland_Destination()
    {
        var manifest = SpatialTransitManifest.CreateDefault();

        var destination = Assert.Single(
            manifest.Destinations.Where(x => x.Id == "singularity-island"));

        Assert.Equal("SINGULARITY ISLAND", destination.Label);
        Assert.Equal("singularity-island", destination.SceneId);
    }
}
