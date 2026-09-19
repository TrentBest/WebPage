using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SpatialSolarSystemManifestTests
{
    [Fact(DisplayName = "Solar system map exposes Singularity Station in Earth orbit")]
    public void SolarSystem_ContainsStationInEarthOrbit()
    {
        var map = SpatialSolarSystemManifest.CreateDefault();
        var station = Assert.Single(map.Facilities, x => x.Id == "singularity-station");
        Assert.Equal("earth", station.ParentBodyId);
        Assert.Equal(SpatialPointKind.Orbit, station.LocationKind);
    }

    [Fact(DisplayName = "Solar system map exposes mixed civilian and logistics traffic")]
    public void SolarSystem_ContainsMixedSpaceTraffic()
    {
        var map = SpatialSolarSystemManifest.CreateDefault();
        Assert.NotEmpty(map.ActivitiesOf(SpatialSpacecraftKind.Freighter));
        Assert.NotEmpty(map.ActivitiesOf(SpatialSpacecraftKind.PassengerShip));
        Assert.NotEmpty(map.ActivitiesOf(SpatialSpacecraftKind.PersonalYacht));
        Assert.Contains(map.Activities, x => x.Kind == SpatialSpacecraftKind.Shuttle &&
                                             x.OriginId == "singularity-station" &&
                                             x.DestinationId == "singularity-orbital-shipyard");
    }
}
