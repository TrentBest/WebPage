using System.Linq;
using TheSingularityWorkshop.Services;
using HubKernel = TheSingularityWorkshop.SingularityHub.SingularityHub;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>
/// Runtime contract tests for the living GUI handoff.
/// These tests deliberately exercise PageFSM rather than Razor so the visual
/// experience cannot silently depend on page-render timing.
/// </summary>
public sealed class LivingGuiRuntimeTests
{
    [Fact(DisplayName = "Page FSM context is explicitly valid at construction")]
    public void PageFsm_ContextStartsValid()
    {
        using var fsm = new PageFSM();

        Assert.True(fsm.Context.IsValid);
        Assert.True(fsm.IsValid);
        Assert.True(fsm.LivingGuiIsValid);
    }

    [Fact(DisplayName = "Living GUI enters population with a centered 200px root actor")]
    public void LivingGui_EntersPopulationWithCenteredRoot()
    {
        using var fsm = new PageFSM();
        fsm.RequestEnter();

        for (var ticks = 0; ticks < 3 && fsm.Context.LivingNodes.Count == 0; ticks++)
            fsm.Update();

        var root = Assert.Single(fsm.Context.LivingNodes);
        Assert.Equal(PageFSM.LivingGuiPopulating, fsm.CurrentState);
        Assert.Equal("G:0", root.Lineage);
        Assert.Equal(0, root.Generation);
        Assert.True(root.IsRoot);
        Assert.Equal(50, root.X);
        Assert.Equal(50, root.Y);
        Assert.Equal(200, root.Size);
        Assert.Equal(PageStateContext.LivingNodePhase.RootGrowth, root.Phase);
        Assert.Equal(0, root.Rotation);
    }

    [Fact(DisplayName = "Living GUI root actor animates from its original size to double before spawning")]
    public void LivingGui_RootAnimatesToDoubleAndThenSpawnsSeed()
    {
        using var fsm = new PageFSM();
        fsm.RequestEnter();

        for (var ticks = 0; ticks < 3 && fsm.Context.LivingNodes.Count == 0; ticks++)
            fsm.Update();

        var root = fsm.Context.LivingNodes[0];

        fsm.Update();
        Assert.Equal(250, root.Size);
        Assert.Equal(PageStateContext.LivingNodePhase.RootGrowth, root.Phase);

        fsm.Update();
        Assert.Equal(300, root.Size);
        Assert.Equal(PageStateContext.LivingNodePhase.RootGrowth, root.Phase);

        fsm.Update();
        Assert.Equal(350, root.Size);
        Assert.Equal(PageStateContext.LivingNodePhase.RootGrowth, root.Phase);

        fsm.Update();
        Assert.Equal(400, root.Size);
        Assert.True(root.SeedDoubled);
        Assert.Equal(PageStateContext.LivingNodePhase.ReproductionPending, root.Phase);

        fsm.Update();

        var child = Assert.Single(fsm.Context.LivingNodes, node => node.Lineage == "G:1");
        Assert.Equal(1, child.Generation);
        Assert.False(child.IsRoot);
        Assert.Equal(40, child.Size);
        Assert.Equal(50, child.X);
        Assert.Equal(50, child.Y);
        Assert.InRange(child.Rotation, -180, 180);
        Assert.NotEqual(0, child.Rotation);
        Assert.InRange(child.TargetX, 10, 90);
        Assert.InRange(child.TargetY, 10, 90);
        Assert.Equal(PageStateContext.LivingNodePhase.SeedFlight, child.Phase);
    }

    [Fact(DisplayName = "Living GUI child reaches default size through flight and scaling, then grows to reproduction size")]
    public void LivingGui_ChildReachesDefaultAndGrows()
    {
        using var fsm = new PageFSM();
        fsm.RequestEnter();

        const int guard = 10_000;
        var ticks = 0;
        while (fsm.Context.LivingNodes.Count < 2 && ticks++ < guard)
            fsm.Update();

        Assert.True(fsm.Context.LivingNodes.Count >= 2);
        var child = fsm.Context.LivingNodes[1];

        ticks = 0;
        while (child.Size < 200 && ticks++ < guard)
            fsm.Update();

        Assert.True(child.Size >= 200, "Living GUI child did not complete flight and scaling within the scheduler guard.");
        Assert.Equal(200, child.Size);
        Assert.True(child.GrowthReady);
        Assert.Equal(PageStateContext.LivingNodePhase.MatureGrowth, child.Phase);

        // Mature growth is intentionally round-robin so the root cannot starve
        // descendants. Advance until this specific child reaches reproduction.
        ticks = 0;
        while (child.Size < 400 && ticks++ < guard)
            fsm.Update();

        Assert.True(child.Size >= 400, "Living GUI child did not receive mature-growth turns within the scheduler guard.");
        Assert.Equal(400, child.Size);
        Assert.Equal(PageStateContext.LivingNodePhase.ReproductionPending, child.Phase);
    }

    [Fact(DisplayName = "Living GUI exposes distinct FSM process groups for every lifecycle phase")]
    public void LivingGui_ExposesDistinctPhaseProcessGroups()
    {
        using var fsm = new PageFSM();

        var groups = new[]
        {
            fsm.LivingGuiProcessingGroup,
            fsm.LivingGuiRootGrowthProcessingGroup,
            fsm.LivingGuiSeedFlightProcessingGroup,
            fsm.LivingGuiMatureGrowthProcessingGroup,
            fsm.LivingGuiReproductionProcessingGroup
        };

        Assert.Equal(groups.Length, groups.Distinct().Count());
        Assert.Contains(fsm.Hub.ProcessGroups, group =>
            group.Name == fsm.LivingGuiProcessingGroup && group.ParentName == fsm.InstanceProcessingGroup);
        Assert.Contains(fsm.Hub.ProcessGroups, group =>
            group.Name == fsm.LivingGuiRootGrowthProcessingGroup && group.ParentName == fsm.LivingGuiProcessingGroup);
        Assert.Contains(fsm.Hub.ProcessGroups, group =>
            group.Name == fsm.LivingGuiSeedFlightProcessingGroup && group.ParentName == fsm.LivingGuiProcessingGroup);
        Assert.Contains(fsm.Hub.ProcessGroups, group =>
            group.Name == fsm.LivingGuiMatureGrowthProcessingGroup && group.ParentName == fsm.LivingGuiProcessingGroup);
        Assert.Contains(fsm.Hub.ProcessGroups, group =>
            group.Name == fsm.LivingGuiReproductionProcessingGroup && group.ParentName == fsm.LivingGuiProcessingGroup);
    }

    [Fact(DisplayName = "Living GUI selects root growth, reproduction, recovery, then seed flight")]
    public void LivingGui_SelectsPhaseInLifecycleOrder()
    {
        using var fsm = new PageFSM();
        fsm.RequestEnter();

        for (var ticks = 0; ticks < 3 && fsm.Context.LivingNodes.Count == 0; ticks++)
            fsm.Update();

        Assert.Equal(LivingGuiFsm.RootGrowthState, fsm.LivingGuiActivePhase);
        fsm.Update();
        Assert.Equal(LivingGuiFsm.RootGrowthState, fsm.LivingGuiActivePhase);
        fsm.Update();
        Assert.Equal(LivingGuiFsm.RootGrowthState, fsm.LivingGuiActivePhase);
        fsm.Update();
        Assert.Equal(LivingGuiFsm.RootGrowthState, fsm.LivingGuiActivePhase);
        fsm.Update();
        Assert.Equal(LivingGuiFsm.ReproductionState, fsm.LivingGuiActivePhase);

        fsm.Update();
        Assert.Equal(LivingGuiFsm.ParentRecoveryState, fsm.LivingGuiActivePhase);

        fsm.Update();
        Assert.Equal(LivingGuiFsm.SeedFlightState, fsm.LivingGuiActivePhase);
    }

    [Fact(DisplayName = "Living GUI lineage names root children and descendants deterministically")]
    public void LivingGui_LineageNaming()
    {
        using var fsm = new PageFSM();
        fsm.RequestEnter();

        const int guard = 10_000;
        var ticks = 0;
        while (fsm.Context.LivingNodes.Count < PageStateContext.PopulationObservationThreshold && ticks++ < guard)
            fsm.Update();

        Assert.True(fsm.Context.LivingNodes.Count >= PageStateContext.PopulationObservationThreshold);
        Assert.Contains(fsm.Context.LivingNodes, node => node.Lineage == "G:0");
        Assert.Contains(fsm.Context.LivingNodes, node => node.Lineage == "G:1");
        Assert.Contains(fsm.Context.LivingNodes, node => node.Lineage == "G:2");
        Assert.Contains(fsm.Context.LivingNodes, node => node.Lineage.StartsWith("G:1-", System.StringComparison.Ordinal));
    }

    [Fact(DisplayName = "Living GUI remains alive after reaching critical mass")]
    public void LivingGui_RemainsAliveAfterCriticalMass()
    {
        using var fsm = new PageFSM();
        fsm.RequestEnter();

        const int guard = 10_000;
        var ticks = 0;
        while (fsm.Context.LivingNodes.Count < PageStateContext.PopulationObservationThreshold && ticks++ < guard)
            fsm.Update();

        Assert.True(fsm.Context.LivingNodes.Count >= PageStateContext.PopulationObservationThreshold, "Living GUI did not reach the population observation threshold within the scheduler guard.");
        Assert.False(fsm.Context.LivingGuiFrozen);
        var populationBefore = fsm.Context.LivingNodes.Count;
        for (var i = 0; i < 200; i++) fsm.Update();
        Assert.True(fsm.Context.LivingNodes.Count > populationBefore, "Living GUI stopped reproducing after reaching the observation threshold.");
        Assert.Contains(fsm.Context.LivingNodes, node => node.Generation > 1);
    }

    [Fact(DisplayName = "PageFSM registers its root and nested process groups with the Hub")]
    public void PageFSM_RegistersRootAndNestedProcessGroupsWithHub()
    {
        using var fsm = new PageFSM();

        Assert.Contains(fsm.Hub.ProcessGroups, group =>
            group.Name == fsm.InstanceProcessingGroup && group.ParentName is null);
        Assert.Contains(fsm.Hub.ProcessGroups, group =>
            group.Name == fsm.LivingGuiProcessingGroup && group.ParentName == fsm.InstanceProcessingGroup);
        Assert.Contains(fsm.Hub.ProcessGroups, group =>
            group.Name == fsm.GravityProcessingGroup && group.ParentName == fsm.InstanceProcessingGroup);
    }

    [Fact(DisplayName = "FSM manager and PageFSM share the registered Hub instance")]
    public void FsmManager_UsesRegisteredHubInstance()
    {
        var hub = new HubKernel();
        using var manager = new FSMManagerService(hub);

        Assert.Same(hub, manager.Page.Hub);
    }
}
