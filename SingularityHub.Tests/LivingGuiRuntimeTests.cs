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
        fsm.Update();

        Assert.NotEqual(PageFSM.Gateway, fsm.CurrentState);
        Assert.NotEmpty(fsm.Context.LivingNodes);
        Assert.Contains(fsm.CurrentState, new[]
        {
            PageFSM.LivingGuiIgnition,
            PageFSM.LivingGuiPopulating
        });
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