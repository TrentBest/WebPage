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
        Assert.False(fsm.Context.MonikerReady);

        fsm.RequestEnter();
        Assert.Equal(PageFSM.GatewayExit, fsm.CurrentState);
        Assert.False(fsm.Context.MonikerReady);

        fsm.Update();
        Assert.Equal(PageFSM.LivingGuiIgnition, fsm.CurrentState);

        fsm.Update();
        Assert.Equal(PageFSM.LivingGuiPopulating, fsm.CurrentState);

        const int populationGuard = 10000;
        var populationTicks = 0;
        while (!fsm.Context.LivingGuiPopulated && populationTicks++ < populationGuard)
        {
            Assert.False(fsm.Context.MonikerReady);
            fsm.Update();
        }

        Assert.True(fsm.Context.LivingGuiPopulated);
        Assert.True(populationTicks < populationGuard, "Living GUI population did not reach critical mass within the test guard.");
        Assert.Equal(PageStateContext.CriticalMass, fsm.Context.LivingNodes.Count);
        Assert.True(fsm.Context.LivingGuiFrozen);
        Assert.True(fsm.Context.MonikerReady);
        Assert.Equal(PageFSM.LivingGuiPopulating, fsm.CurrentState);

        fsm.Update();
        Assert.Equal(PageFSM.MonikerReveal, fsm.CurrentState);
        Assert.True(fsm.Context.MonikerReady);
        Assert.True(fsm.Context.LivingGuiFrozen);

        fsm.Update();
        Assert.Equal(PageFSM.Gravity, fsm.CurrentState);
        Assert.True(fsm.Context.MonikerReady);
        Assert.False(fsm.Context.LivingGuiFallen);

        const int gravityGuard = 1000;
        var gravityTicks = 0;
        while (!fsm.Context.LivingGuiFallen && gravityTicks++ < gravityGuard)
        {
            Assert.Equal(PageFSM.Gravity, fsm.CurrentState);
            Assert.True(fsm.Context.MonikerReady);
            fsm.Update();
        }

        Assert.True(gravityTicks < gravityGuard, "Living GUI did not fall away within the test guard.");
        Assert.True(fsm.Context.LivingGuiFallen);

        // FSM_API evaluates the Gravity -> Dissipating transition on the same
        // heartbeat that marks the living GUI as fallen. Therefore the observable
        // post-fall state is already DISSIPATING; no extra heartbeat is required.
        Assert.Equal(PageFSM.LivingGuiDissipating, fsm.CurrentState);

        for (var tick = 1; tick < 91; tick++)
        {
            fsm.Update();
            Assert.Equal(PageFSM.LivingGuiDissipating, fsm.CurrentState);
            Assert.True(fsm.Context.MonikerReady);
        }

        fsm.Update();
        Assert.Equal(PageFSM.NavigationArrival, fsm.CurrentState);
        Assert.True(fsm.Context.MonikerReady);
    }

    [Fact(DisplayName = "Incremental Unit Test 07 — disposing PageFSM unregisters all runtime handles")]
    public void IncrementalUnitTest07_DisposingPageFSMUnregistersRuntimeHandles()
    {
        string pageGroup;
        string livingGuiGroup;
        string gravityGroup;

        using (var fsm = new PageFSM())
        {
            pageGroup = fsm.InstanceProcessingGroup;
            livingGuiGroup = fsm.LivingGuiProcessingGroup;
            gravityGroup = fsm.GravityProcessingGroup;

            Assert.Equal(1, fsm_API.Internal.GetFSMHandleCountInGroup(pageGroup));
            Assert.Equal(1, fsm_API.Internal.GetFSMHandleCountInGroup(livingGuiGroup));
            Assert.Equal(1, fsm_API.Internal.GetFSMHandleCountInGroup(gravityGroup));
        }

        Assert.Equal(0, fsm_API.Internal.GetFSMHandleCountInGroup(pageGroup));
        Assert.Equal(0, fsm_API.Internal.GetFSMHandleCountInGroup(livingGuiGroup));
        Assert.Equal(0, fsm_API.Internal.GetFSMHandleCountInGroup(gravityGroup));
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

    [Fact(DisplayName = "Incremental Unit Test 09 — moniker remains hidden until exactly 100 nodes")]
    public void IncrementalUnitTest09_MonikerRemainsHiddenUntilExactly100Nodes()
    {
        using var fsm = new PageFSM();

        fsm.RequestEnter();
        fsm.Update();
        fsm.Update();

        const int guard = 10000;
        var ticks = 0;
        while (!fsm.Context.LivingGuiPopulated && ticks++ < guard)
        {
            Assert.True(fsm.Context.LivingNodes.Count < PageStateContext.CriticalMass);
            Assert.False(fsm.Context.MonikerReady);
            fsm.Update();
        }

        Assert.True(ticks < guard, "Living GUI population did not reach critical mass within the test guard.");
        Assert.Equal(PageStateContext.CriticalMass, fsm.Context.LivingNodes.Count);
        Assert.True(fsm.Context.LivingGuiFrozen);
        Assert.True(fsm.Context.MonikerReady);
        Assert.Equal(PageFSM.LivingGuiPopulating, fsm.CurrentState);

        fsm.Update();
        Assert.Equal(PageFSM.MonikerReveal, fsm.CurrentState);
        Assert.True(fsm.Context.MonikerReady);
    }
}
