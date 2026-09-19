using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SpatialSolarSystemDataManifestTests
{
    [Fact]
    public void NasaManifest_ContainsThePlanetaryReferenceSet()
    {
        var data = SpatialSolarSystemDataManifest.Nasa;

        Assert.Equal("NASA NSSDCA Planetary Fact Sheet - Metric", data.SourceName);
        Assert.Equal(9, data.Bodies.Count);
        Assert.Equal(12756, data.Find("earth")!.Value.DiameterKm);
        Assert.Equal(149.6, data.Find("earth")!.Value.MeanDistanceFromSunMillionKm);
        Assert.Equal(365.2, data.Find("earth")!.Value.OrbitalPeriodDays);
    }

    [Fact]
    public void MapScope_ExposesSolarSystem()
    {
        Assert.Contains(SpatialMapScope.SolarSystem, Enum.GetValues<SpatialMapScope>());
    }
}
