using TheSingularityWorkshop.Services;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>
/// Incremental heartbeat 0.0.65.
/// The Living GUI must report IDLE when it has no lifecycle work, including
/// before ignition and after the population has intentionally frozen.
/// </summary>
public sealed class IncrementalVersion065Tests
{
    private const string Version = "0.0.65";

    [ArchitectureTest(0, 0, 65)]
    [Fact(DisplayName = "0.00.065 — Idle_Living_GUI_Does_Not_Report_Parent_Recovery")]
    public void IdleLivingGuiDoesNotReportParentRecovery()
    {
        using var fsm = new PageFSM();

        Assert.Equal(Version, Version);
        Assert.Equal(0, fsm.Context.LivingNodes.Count);
        Assert.Equal(LivingGuiFsm.LifecycleState, fsm.LivingGuiState);
        Assert.Equal(LivingGuiFsm.IdleState, fsm.LivingGuiActivePhase);
        Assert.True(fsm.LivingGuiIsValid);
    }

    [Fact(DisplayName = "Frozen Living GUI does not retain a stale active phase")]
    public void FrozenLivingGuiDoesNotRetainStaleActivePhase()
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
        Assert.Equal(LivingGuiFsm.IdleState, fsm.LivingGuiActivePhase);
    }
}
