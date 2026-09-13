using TheSingularityWorkshop.SingularityHub;
using Xunit;

namespace SingularityHub.Tests;

public sealed class IncrementalVersion066Tests
{
    private const string Version = "0.0.66";

    [ArchitectureTest(0, 0, 66)]
    [Fact(DisplayName = "0.00.066 — Hub_Route_Carries_Stable_Experience_Destination")]
    public void HubRouteCarriesStableExperienceDestination()
    {
        var route = new SingularityRoute(101, "/workshop", "WORKSHOP", 5001);

        Assert.Equal(Version, Version);
        Assert.Equal(101UL, route.RouteId);
        Assert.Equal("/workshop", route.Path);
        Assert.Equal("WORKSHOP", route.Tab);
        Assert.Equal(5001UL, route.ExperienceId);
    }
}
