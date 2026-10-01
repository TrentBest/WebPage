using HubKernel = TheSingularityWorkshop.SingularityHub.SingularityHub;
using TheSingularityWorkshop.SingularityHub;
using Xunit;

namespace SingularityHub.Tests;

public sealed class HubConfigurationTests
{
    [Fact(DisplayName = "Hub configuration renders the tabs declared for its current level")]
    public void CurrentLevelIsConfigurationDriven()
    {
        var configuration = new SingularityHubConfiguration();
        var definition = new SingularityHubDefinition(
            100,
            "/workshop",
            "WORKSHOP",
            new[]
            {
                new HubTabDefinition(2, "AI", 1, HubTabTargetKind.Tool, "/workshop/ai", 200),
                new HubTabDefinition(1, "Explore", 0, HubTabTargetKind.Experience, "/workshop/explore", 300)
            });

        Assert.True(configuration.Register(definition));
        Assert.True(configuration.TryResolve("/WORKSHOP", out var resolved));
        Assert.Equal("WORKSHOP", resolved!.Title);
        Assert.Equal(new[] { "Explore", "AI" }, resolved.Tabs.Select(tab => tab.Label));
    }

    [Fact(DisplayName = "A Hub tab can target another configured Hub")]
    public void HubTabCanNestAnotherHub()
    {
        var configuration = new SingularityHubConfiguration();

        var child = new SingularityHubDefinition(
            200,
            "/workshop/ai",
            "AI",
            new[]
            {
                new HubTabDefinition(3, "Exchange", 0, HubTabTargetKind.Tool, "/workshop/ai/exchange", 400)
            });

        var parent = new SingularityHubDefinition(
            100,
            "/workshop",
            "WORKSHOP",
            new[]
            {
                new HubTabDefinition(2, "AI", 0, HubTabTargetKind.Hub, "/workshop/ai", child.Id)
            });

        Assert.True(configuration.Register(parent));
        Assert.True(configuration.Register(child));
        Assert.True(parent.Tabs[0].IsNestedHub);
        Assert.Equal(child.Id, parent.Tabs[0].TargetId);
        Assert.True(configuration.TryResolve(parent.Tabs[0].Route, out var nested));
        Assert.Same(child, nested);
    }

    [Fact(DisplayName = "A tab can declare a provider as its acquisition source")]
    public void TabCanDeclareAcquisitionSource()
    {
        var tab = new HubTabDefinition(
            10,
            "AI Exchange",
            0,
            HubTabTargetKind.Tool,
            "/workshop/ai",
            acquisition: new HubTabAcquisition(
                HubTabAcquisitionKind.Provider,
                0xA100UL,
                "ai.exchange"));

        Assert.True(tab.IsAcquired);
        Assert.Equal(0xA100UL, tab.Acquisition!.Value.SourceId);
        Assert.Equal("ai.exchange", tab.Acquisition.Value.Selector);
    }

    [Fact(DisplayName = "Hub configuration rejects duplicate tab placement")]
    public void DuplicatePlacementIsRejected()
    {
        Assert.Throws<ArgumentException>(() =>
            new SingularityHubDefinition(
                100,
                "/workshop",
                "WORKSHOP",
                new[]
                {
                    new HubTabDefinition(1, "A", 0, HubTabTargetKind.Tool, "/a", 10),
                    new HubTabDefinition(2, "B", 0, HubTabTargetKind.Tool, "/b", 20)
                }));
    }

    [Fact(DisplayName = "Hub runtime exposes the same configuration boundary as its routing and arbitration")]
    public void RuntimeHubExposesConfiguration()
    {
        var hub = new HubKernel(_ => { });

        Assert.NotNull(hub.Configuration);
        Assert.Same(hub.Configuration, ((ISingularityHub)hub).Configuration);
    }
}
