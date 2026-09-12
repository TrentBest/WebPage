using TheSingularityWorkshop.Services;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>
/// Runtime contract tests for the living GUI handoff.
/// These tests deliberately exercise PageFSM rather than Razor so the visual
/// experience cannot silently depend on page-render timing.
/// </summary>
public sealed class LivingGuiRuntimeTests
{
    [Fact(DisplayName = "Living GUI enters population after gateway request")]
    public void LivingGui_EntersPopulationAfterGatewayRequest()
    {
        using var fsm = new PageFSM();

        Assert.Equal(PageFSM.Gateway, fsm.CurrentState);
        Assert.Empty(fsm.Context.LivingNodes);

        fsm.RequestEnter();

        for (var ticks = 0; ticks < 3 && fsm.Context.LivingNodes.Count == 0; ticks++)
            fsm.Update();

        Assert.NotEqual(PageFSM.Gateway, fsm.CurrentState);
        Assert.NotEmpty(fsm.Context.LivingNodes);
        Assert.Equal("G:0", fsm.Context.LivingNodes[0].Lineage);
        Assert.Contains(fsm.CurrentState, new[]
        {
            PageFSM.LivingGuiIgnition,
            PageFSM.LivingGuiPopulating
        });
    }

    [Fact(DisplayName = "Living GUI seed doubles, travels, reaches default size, then grows")]
    public void LivingGui_SeedLifecycle()
    {
        using var fsm = new PageFSM();
        fsm.RequestEnter();

        for (var ticks = 0; ticks < 3 && fsm.Context.LivingNodes.Count == 0; ticks++)
            fsm.Update();

        var root = fsm.Context.LivingNodes[0];
        var startX = root.X;
        var startY = root.Y;
        var startSize = root.Size;

        fsm.Update();
        Assert.Equal(startSize * 2, root.Size);
        Assert.True(root.SeedDoubled);

        var flightGuard = root.SeedFlightDuration + 2;
        for (var ticks = 0; ticks < flightGuard && !root.GrowthReady; ticks++)
            fsm.Update();

        Assert.True(root.GrowthReady);
        Assert.Equal(48, root.Size);
        Assert.NotEqual((startX, startY), (root.X, root.Y));

        fsm.Update();
        Assert.Equal(72, root.Size);

        fsm.Update();
        Assert.Equal(96, root.Size);
    }

    [Fact(DisplayName = "Living GUI lineage names root children and descendants deterministically")]
    public void LivingGui_LineageNaming()
    {
        using var fsm = new PageFSM();
        fsm.RequestEnter();

        const int guard = 10_000;
        var ticks = 0;
        while (fsm.Context.LivingNodes.Count < 100 && ticks++ < guard)
            fsm.Update();

        Assert.Equal(PageStateContext.CriticalMass, fsm.Context.LivingNodes.Count);
        Assert.Contains(fsm.Context.LivingNodes, node => node.Lineage == "G:0");
        Assert.Contains(fsm.Context.LivingNodes, node => node.Lineage == "G:1");
        Assert.Contains(fsm.Context.LivingNodes, node => node.Lineage == "G:2");
        Assert.Contains(fsm.Context.LivingNodes, node => node.Lineage.StartsWith("G:1-", StringComparison.Ordinal));
    }

    [Fact(DisplayName = "Living GUI reaches exact critical mass through its isolated scheduler")]
    public void LivingGui_ReachesExactCriticalMass()
    {
        using var fsm = new PageFSM();
        fsm.RequestEnter();

        const int guard = 10_000;
        var ticks = 0;
        while (!fsm.Context.LivingGuiPopulated && ticks++ < guard)
            fsm.Update();

        Assert.True(fsm.Context.LivingGuiPopulated, "Living GUI did not reach critical mass within the scheduler guard.");
        Assert.True(fsm.Context.LivingGuiFrozen);
        Assert.Equal(PageStateContext.CriticalMass, fsm.Context.LivingNodes.Count);
        Assert.True(fsm.Context.MonikerReady);
    }
}
