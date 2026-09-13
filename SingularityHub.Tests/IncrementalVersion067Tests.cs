using TheSingularityWorkshop.SingularityHub;
using Xunit;

namespace SingularityHub.Tests;

public sealed class IncrementalVersion067Tests
{
    private const string Version = "0.0.67";

    [ArchitectureTest(0, 0, 67)]
    [Fact(DisplayName = "0.00.067 — SingularityHub_Exposes_Routing")]
    public void SingularityHubExposesRouting()
    {
        ISingularityHub hub = new TheSingularityWorkshop.SingularityHub.SingularityHub(_ => { });

        Assert.Equal(Version, Version);
        Assert.NotNull(hub.Routing);
        Assert.Empty(hub.Routing.Routes);
    }

    [Fact(DisplayName = "SingularityHub routing registers and resolves destinations")]
    public void RoutingRegistersAndResolves()
    {
        ISingularityHub hub = new TheSingularityWorkshop.SingularityHub.SingularityHub(_ => { });
        var route = new SingularityRoute(101, "/library", "LIBRARY", 7001);

        Assert.True(hub.Routing.Register(route));
        Assert.True(hub.Routing.TryResolve("/library", out var resolved));
        Assert.Equal(route, resolved);
    }
}
