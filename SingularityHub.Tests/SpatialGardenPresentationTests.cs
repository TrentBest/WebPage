using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SpatialGardenPresentationTests
{
    [Fact(DisplayName = "Incremental Unit Test 65 — garden is a first-visit presentation state rather than a permanent replacement for Explore")]
    public void GardenHasAThresholdIdentity()
    {
        var garden = SpatialGardenManifest.CreateDefault();

        Assert.NotEqual("workshop", garden.Id);
        Assert.Contains("Experiences", garden.Purpose);
        Assert.Equal(3, garden.Gates.Count);
    }
}
