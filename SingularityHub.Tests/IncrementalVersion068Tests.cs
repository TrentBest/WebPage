using TheSingularityWorkshop.SingularityHub;
using Xunit;

namespace SingularityHub.Tests;

public sealed class IncrementalVersion068Tests
{
    private const string Version = "0.0.68";

    [ArchitectureTest(0, 0, 68)]
    [Fact(DisplayName = "0.00.068 — Routing_Rejects_Duplicate_Paths")]
    public void RoutingRejectsDuplicatePaths()
    {
        ISingularityHub hub = new SingularityHub(_ => { });
        var first = new SingularityRoute(1, "/about", "ABOUT", 8001);
        var duplicatePath = new SingularityRoute(2, "/about", "OTHER", 8002);

        Assert.Equal(Version, Version);
        Assert.True(hub.Routing.Register(first));
        Assert.False(hub.Routing.Register(duplicatePath));
        Assert.Single(hub.Routing.Routes);
    }
}
