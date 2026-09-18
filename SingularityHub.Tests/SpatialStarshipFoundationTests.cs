using System.Linq;
using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>Heartbeat tests for the Station-to-Starship composition boundary.</summary>
public sealed class SpatialStarshipFoundationTests
{
    [Fact]
    public void Starship_ContainsCrewStations_AndScaleChangingVisualizations()
    {
        var ship = SpatialStarshipManifest.CreateDefault();

        Assert.Equal("SINGULARITY STARSHIP", ship.Name);
        Assert.Contains(ship.Stations, x => x.Id == "bridge");
        Assert.Contains(ship.Stations, x => x.Id == "engineering");
        Assert.Contains(ship.Stations, x => x.Id == "science");
        Assert.Equal(4, ship.Visualizations.Count);
        Assert.Equal("UNIVERSAL", ship.Visualizations.Last().Scale);
    }

    [Fact]
    public void Starship_EnvironmentSeparatesObservationFromOperation()
    {
        var bridge = SpatialStarshipManifest.CreateDefault()
            .Stations
            .Single(x => x.Id == "bridge");

        Assert.Contains("flight controls", bridge.Interactables);
        Assert.Contains("sensors", bridge.Interactables);
        Assert.Contains("navigation", bridge.CrewDomain);
    }

    [Fact]
    public void Starship_CanFilterStationsByCrewDomain()
    {
        var ship = SpatialStarshipManifest.CreateDefault();

        var engineering = ship.ForCrewDomain("ENGINEERING");

        Assert.Single(engineering);
        Assert.Equal("engineering", engineering[0].Id);
    }
}
