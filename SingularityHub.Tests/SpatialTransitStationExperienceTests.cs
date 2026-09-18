using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>Heartbeat tests for the Workshop's Grand Central transit contract.</summary>
public sealed class SpatialTransitStationExperienceTests
{
    [Fact]
    public void TransitManifest_Exposes_Distinct_Platform_Destinations()
    {
        var manifest = SpatialTransitManifest.CreateDefault();

        Assert.Equal("workshop-tram", manifest.Id);
        Assert.Equal(4, manifest.Destinations.Count);
        Assert.Equal(manifest.Destinations.Count, manifest.Destinations.Select(x => x.Id).Distinct().Count());
        Assert.All(manifest.Destinations, destination =>
        {
            Assert.False(string.IsNullOrWhiteSpace(destination.Label));
            Assert.False(string.IsNullOrWhiteSpace(destination.SceneId));
        });
    }

    [Fact]
    public void TransitManifest_Provides_Track_Platform_And_Timing()
    {
        var manifest = SpatialTransitManifest.CreateDefault();

        Assert.True(manifest.TrackCenterline.Count >= 2);
        Assert.True(manifest.PlatformBounds.Width > 0);
        Assert.True(manifest.PlatformBounds.Height > 0);
        Assert.True(manifest.Timing.CycleSeconds > 0);
        Assert.Equal(6, manifest.Passengers.Count);
    }
}
