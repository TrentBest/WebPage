using TheSingularityWorkshop.Services;
using TheSingularityWorkshop.SingularityHub;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>
/// Runtime contract tests for the living GUI handoff.
/// These tests deliberately exercise PageFSM rather than Razor so the visual
/// experience cannot silently depend on page-render timing.
/// </summary>
public sealed class LivingGuiRuntimeTests
{
    [Fact(DisplayName = "Living GUI enters population with a centered 200px root")]
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
    }

    [Fact(DisplayName = "Living GUI root doubles immediately and then spawns a safe rooted child")]
    public void LivingGui_RootDoublesAndSpawnsRootedChild()
    {
        using var fsm = new PageFSM();
        fsm.RequestEnter();

        for (var ticks = 0; ticks < 3 && fsm.Context.LivingNodes.Count == 0; ticks++)
            fsm.Update();

        var root = fsm.Context.LivingNodes[0];
        fsm.Update();
        Assert.Equal(400, root.Size);
        Assert.True(root.SeedDoubled);

        fsm.Update();

        var child = Assert.Single(fsm.Context.LivingNodes, node => node.Lineage == "G:1");
        Assert.Equal(1, child.Generation);
        Assert.False(child.IsRoot);
        Assert.Equal(40, child.Size);
        Assert.InRange(child.X, 10, 90);
        Assert.InRange(child.Y, 10, 90);
        Assert.Equal(50, root.X);
        Assert.Equal(50, root.Y);
    }

    [Fact(DisplayName = "Living GUI child reaches default size quickly, then grows to reproduction size")]
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

        fsm.Update();
        Assert.Equal(200, child.Size);
        Assert.True(child.GrowthReady);

        fsm.Update();
        Assert.Equal(250, child.Size);

        fsm.Update();
        Assert.Equal(300, child.Size);

        fsm.Update();
        Assert.Equal(350, child.Size);

        fsm.Update();
        Assert.Equal(400, child.Size);
    }

    [Fact(DisplayName = "Living GUI exposes distinct FSM process groups for every lifecycle phase")]
    public void LivingGui_ExposesDistinctPhaseProcessGroups()
    {
        using var fsm = new PageFSM();

        var groups = new[]
        {
            fsm.LivingGuiProcessingGroup,
            fsm.LivingGuiRootGrowthProcessingGroup,
            fsm.LivingGuiChildRootingProcessingGroup,
            fsm.LivingGuiMatureGrowthProcessingGroup,
            fsm.LivingGuiReproductionProcessingGroup
        };

        Assert.Equal(groups.Length, groups.Distinct().Count());
        Assert.Contains(fsm.Hub.ProcessGroups, group =>
            group.Name == fsm.LivingGuiProcessingGroup && group.ParentName == fsm.InstanceProcessingGroup);
        Assert.Contains(fsm.Hub.ProcessGroups, group =>
            group.Name == fsm.LivingGuiRootGrowthProcessingGroup && group.ParentName == fsm.LivingGuiProcessingGroup);
        Assert.Contains(fsm.Hub.ProcessGroups, group =>
            group.Name == fsm.LivingGuiChildRootingProcessingGroup && group.ParentName == fsm.LivingGuiProcessingGroup);
        Assert.Contains(fsm.Hub.ProcessGroups, group =>
            group.Name == fsm.LivingGuiMatureGrowthProcessingGroup && group.ParentName == fsm.LivingGuiProcessingGroup);
        Assert.Contains(fsm.Hub.ProcessGroups, group =>
            group.Name == fsm.LivingGuiReproductionProcessingGroup && group.ParentName == fsm.LivingGuiProcessingGroup);
    }

    [Fact(DisplayName = "Living GUI selects root growth before child rooting and mature growth")]
    public void LivingGui_SelectsPhaseInLifecycleOrder()
    {
        using var fsm = new PageFSM();
        fsm.RequestEnter();

        for (var ticks = 0; ticks < 3 && fsm.Context.LivingNodes.Count == 0; ticks++)
            fsm.Update();

        Assert.Equal(LivingGuiFsm.RootGrowthState, fsm.LivingGuiActivePhase);
        fsm.Update();
        Assert.Equal(LivingGuiFsm.ReproductionState, fsm.LivingGuiActivePhase);

        fsm.Update();
        Assert.Equal(LivingGuiFsm.ChildRootingState, fsm.LivingGuiActivePhase);
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
        using var hub = new SingularityHub();
        using var manager = new FSMManagerService(hub);

        Assert.Same(hub, manager.Page.Hub);
    }
}
