using System;
using Xunit;
using TheSingularityWorkshop.Services;
using fsm_API = TheSingularityWorkshop.FSM_API.FSM_API;

namespace SingularityHub.Tests;

public sealed class IdleExperienceArchitectureTests
{
    [Fact(DisplayName = "Unit 04 — PageFSM follows exact presentation sequence")]
    public void PageFSM_FollowsExactPresentationSequence()
    {
        using var fsm = new PageFSM();

        Assert.Equal(PageFSM.Gateway, fsm.CurrentState);

        fsm.RequestEnter();
        Assert.Equal(PageFSM.GatewayExit, fsm.CurrentState);

        fsm.Update();
        Assert.Equal(PageFSM.LivingGuiIgnition, fsm.CurrentState);

        fsm.Update();
        Assert.Equal(PageFSM.LivingGuiPopulating, fsm.CurrentState);

        while (!fsm.Context.LivingGuiPopulated)
        {
            fsm.Update();
        }

        Assert.Equal(100, fsm.Context.LivingNodes.Count);
        Assert.Equal(PageFSM.MonikerReveal, fsm.CurrentState);

        // FSM_API transitions into the next state first; its scheduler owns the
        // following heartbeat's OnEnter lifecycle. The first Gravity heartbeat
        // therefore establishes StateTicks == 1 and makes the moniker visible.
        fsm.Update();
        Assert.Equal(PageFSM.Gravity, fsm.CurrentState);
        Assert.False(fsm.Context.MonikerReady);

        fsm.Update();
        Assert.Equal(PageFSM.Gravity, fsm.CurrentState);
        Assert.True(fsm.Context.MonikerReady);
        Assert.Equal(1, fsm.Context.StateTicks);

        for (var tick = 2; tick <= 90; tick++)
        {
            fsm.Update();
            Assert.Equal(PageFSM.Gravity, fsm.CurrentState);
            Assert.Equal(tick, fsm.Context.StateTicks);
        }

        fsm.Update();
        Assert.Equal(PageFSM.NavigationArrival, fsm.CurrentState);
        Assert.Equal(91, fsm.Context.StateTicks);
    }

    [Fact(DisplayName = "Incremental Unit Test 07 — disposing PageFSM unregisters its runtime handle")]
    public void IncrementalUnitTest07_DisposingPageFSMUnregistersRuntimeHandle()
    {
        string processingGroup;

        using (var fsm = new PageFSM())
        {
            processingGroup = fsm.InstanceProcessingGroup;
            Assert.Equal(1, fsm_API.Internal.GetFSMHandleCountInGroup(processingGroup));
        }

        Assert.Equal(0, fsm_API.Internal.GetFSMHandleCountInGroup(processingGroup));
    }

    [Fact(DisplayName = "Incremental Unit Test 08 — PageFSM Update advances only its owning handle")]
    public void IncrementalUnitTest08_PageFSMUpdateAdvancesOnlyItsOwningHandle()
    {
        using var first = new PageFSM();
        using var second = new PageFSM();

        Assert.Equal(PageFSM.Gateway, first.CurrentState);
        Assert.Equal(PageFSM.Gateway, second.CurrentState);

        first.RequestEnter();
        Assert.Equal(PageFSM.GatewayExit, first.CurrentState);
        Assert.Equal(PageFSM.Gateway, second.CurrentState);

        first.Update();
        Assert.Equal(PageFSM.LivingGuiIgnition, first.CurrentState);
        Assert.Equal(PageFSM.Gateway, second.CurrentState);
    }

    [Fact(DisplayName = "Incremental Unit Test 09 — moniker remains hidden until gravity")]
    public void IncrementalUnitTest09_MonikerRemainsHiddenUntilGravity()
    {
        using var fsm = new PageFSM();

        fsm.RequestEnter();
        fsm.Update();
        fsm.Update();

        while (!fsm.Context.LivingGuiPopulated)
        {
            Assert.False(fsm.Context.MonikerReady);
            fsm.Update();
        }

        Assert.False(fsm.Context.MonikerReady);
        Assert.Equal(PageFSM.MonikerReveal, fsm.CurrentState);

        fsm.Update();
        Assert.Equal(PageFSM.Gravity, fsm.CurrentState);
        Assert.False(fsm.Context.MonikerReady);

        // OnEnter for Gravity runs on the scheduler's next heartbeat.
        fsm.Update();
        Assert.Equal(PageFSM.Gravity, fsm.CurrentState);
        Assert.True(fsm.Context.MonikerReady);
    }
}
