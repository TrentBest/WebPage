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
        Assert.True(populationTicks < populationGuard, "Living GUI population did not reach the observation threshold within the test guard.");
        Assert.True(fsm.Context.LivingNodes.Count >= PageStateContext.PopulationObservationThreshold);
        Assert.False(fsm.Context.LivingGuiFrozen);
        Assert.False(fsm.Context.MonikerReady);
        Assert.Equal(PageFSM.LivingGuiPopulating, fsm.CurrentState);

        fsm.Update();
        Assert.Equal(PageFSM.MonikerReveal, fsm.CurrentState);
        Assert.False(fsm.Context.MonikerReady);
        Assert.False(fsm.Context.LivingGuiFrozen);

        // Gravity is the explicit freeze/reveal boundary. FSM_API applies state-entry
        // actions on the following heartbeat, so the Moniker becomes visible only when
        // Gravity has actually entered and the organisms are frozen.
        fsm.Update();
        Assert.Equal(PageFSM.Gravity, fsm.CurrentState);
        fsm.Update();
        Assert.Equal(PageFSM.Gravity, fsm.CurrentState);
        Assert.True(fsm.Context.MonikerReady);
        Assert.True(fsm.Context.LivingGuiFrozen);
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
        Assert.Equal(PageFSM.LivingGuiDissipating, fsm.CurrentState);

        var presentationGuard = 120;
        while (fsm.CurrentState == PageFSM.LivingGuiDissipating && presentationGuard-- > 0)
        {
            Assert.True(fsm.Context.MonikerReady);
            fsm.Update();
        }

        Assert.Equal(PageFSM.NavigationArrival, fsm.CurrentState);
        Assert.True(fsm.Context.MonikerReady);
        Assert.InRange(
            fsm.Context.TotalTicks - fsm.Context.MonikerPresentationStartTick,
            91,
            92);
    }

    [Fact(DisplayName = "First contact hands Workshop entry to the landing runtime instead of presenting the moniker itself")]
    public void FirstContact_GatewayEntryHandsOffWithoutMoniker()
    {
        using var firstContact = new FirstContactFsm();
        firstContact.Start();

        for (var i = 0; i < 200 && firstContact.CurrentState != "Gateway"; i++)
            firstContact.Update();

        Assert.Equal("Gateway", firstContact.CurrentState);

        firstContact.RequestEntry();
        firstContact.Update();

        Assert.Equal("Landing", firstContact.CurrentState);
        Assert.NotEqual("Moniker", firstContact.CurrentState);
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

    [Fact(DisplayName = "Incremental Unit Test 09 — moniker waits for the population threshold and Gravity") ]
    public void IncrementalUnitTest09_MonikerWaitsForPopulationThresholdAndGravity()
    {
        using var fsm = new PageFSM();

        fsm.RequestEnter();
        fsm.Update();
        fsm.Update();

        const int guard = 10000;
        var ticks = 0;
        while (!fsm.Context.LivingGuiPopulated && ticks++ < guard)
        {
            Assert.True(fsm.Context.LivingNodes.Count < PageStateContext.PopulationObservationThreshold);
            Assert.False(fsm.Context.MonikerReady);
            fsm.Update();
        }

        Assert.True(ticks < guard, "Living GUI did not reach the population observation threshold within the test guard.");
        Assert.True(fsm.Context.LivingNodes.Count >= PageStateContext.PopulationObservationThreshold);
        Assert.False(fsm.Context.LivingGuiFrozen);
        Assert.False(fsm.Context.MonikerReady);
        Assert.Equal(PageFSM.LivingGuiPopulating, fsm.CurrentState);

        fsm.Update();
        Assert.Equal(PageFSM.MonikerReveal, fsm.CurrentState);
        Assert.False(fsm.Context.MonikerReady);
        Assert.False(fsm.Context.LivingGuiFrozen);

        fsm.Update();
        Assert.Equal(PageFSM.Gravity, fsm.CurrentState);
        fsm.Update();
        Assert.True(fsm.Context.LivingGuiFrozen);
        Assert.True(fsm.Context.MonikerReady);
    }

    [Fact(DisplayName = "Incremental Unit Test 10 — moniker presentation defaults to three seconds and accepts a parameter")]
    public void IncrementalUnitTest10_MonikerPresentationDurationIsConfigurable()
    {
        using var defaultFsm = new PageFSM();
        using var customFsm = new PageFSM(monikerPresentationDuration: TimeSpan.FromSeconds(5));

        Assert.Equal(TimeSpan.FromSeconds(3), defaultFsm.MonikerPresentationDuration);
        Assert.Equal(TimeSpan.FromSeconds(5), customFsm.MonikerPresentationDuration);

        defaultFsm.RequestEnter();
        defaultFsm.Update();
        defaultFsm.Update();
        for (var ticks = 0; !defaultFsm.Context.LivingGuiPopulated && ticks < 10000; ticks++)
            defaultFsm.Update();
        Assert.True(defaultFsm.Context.LivingGuiPopulated);
        defaultFsm.Update();
        Assert.Equal(PageFSM.MonikerReveal, defaultFsm.CurrentState);
        Assert.False(defaultFsm.Context.NavigationReady);

        defaultFsm.SignalNavigationReady();
        defaultFsm.Update();
        Assert.Equal(PageFSM.Gravity, defaultFsm.CurrentState);

        for (var ticks = 0; !defaultFsm.Context.LivingGuiFallen && ticks < 1000; ticks++)
            defaultFsm.Update();
        Assert.True(defaultFsm.Context.LivingGuiFallen);
        Assert.Equal(PageFSM.LivingGuiDissipating, defaultFsm.CurrentState);
        defaultFsm.Update();
        Assert.Equal(PageFSM.NavigationArrival, defaultFsm.CurrentState);
    }
    [Fact(DisplayName = "Workshop moniker persists after the living GUI falls away")]
    public void WorkshopMonikerPersistsAfterLivingGuiDissipates()
    {
        using var fsm = new PageFSM();

        fsm.RequestEnter();
        fsm.Update();
        fsm.Update();

        for (var ticks = 0; !fsm.Context.LivingGuiFallen && ticks < 10000; ticks++)
            fsm.Update();
        Assert.True(fsm.Context.LivingGuiFallen);

        for (var ticks = 0; fsm.CurrentState != PageFSM.Running && ticks < 10000; ticks++)
            fsm.Update();
        Assert.Equal(PageFSM.Running, fsm.CurrentState);

        Assert.True(fsm.Context.MonikerReady);
        Assert.True(fsm.Context.LivingGuiFallen);
        Assert.Equal(PageFSM.Running, fsm.CurrentState);
    }

}
