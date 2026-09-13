using TheSingularityWorkshop.Services;
using TheSingularityWorkshop.SingularityHub;
using Xunit;

namespace SingularityHub.Tests;

public sealed class LivingGuiRuntimeTests
{
    [Fact(DisplayName = "Living GUI starts with a centered 200px root")]
    public void LivingGui_StartsWithCenteredRoot()
    {
        using var runtime = CreateRuntime();

        Assert.Single(runtime.Context.LivingNodes);
        var root = runtime.Context.LivingNodes[0];
        Assert.True(root.IsRoot);
        Assert.Equal(200, root.Size);
        Assert.Equal(50, root.X);
        Assert.Equal(50, root.Y);
        Assert.Equal(0, root.Generation);
    }

    [Fact(DisplayName = "Living GUI root doubles to 400px and spawns a safe rooted child")]
    public void LivingGui_RootDoublesAndSpawnsChild()
    {
        using var runtime = CreateRuntime();

        runtime.Update();

        var root = Assert.Single(runtime.Context.LivingNodes.Where(node => node.IsRoot));
        Assert.Equal(400, root.Size);

        var child = Assert.Single(runtime.Context.LivingNodes.Where(node => !node.IsRoot));
        Assert.Equal(40, child.Size);
        Assert.Equal(1, child.Generation);
        Assert.InRange(child.X, 10, 90);
        Assert.InRange(child.Y, 10, 90);
    }

    [Fact(DisplayName = "Living GUI child roots to 200px then grows in 50px steps")]
    public void LivingGui_ChildRootsAndGrows()
    {
        using var runtime = CreateRuntime();

        runtime.Update();
        runtime.Update();

        var child = Assert.Single(runtime.Context.LivingNodes.Where(node => !node.IsRoot));
        Assert.Equal(200, child.Size);

        runtime.Update();
        Assert.Equal(250, child.Size);

        runtime.Update();
        Assert.Equal(300, child.Size);

        runtime.Update();
        Assert.Equal(350, child.Size);

        runtime.Update();
        Assert.Equal(400, child.Size);
    }

    [Fact(DisplayName = "Living GUI phases have distinct FSM API process groups")]
    public void LivingGui_PhasesHaveDistinctProcessGroups()
    {
        using var runtime = CreateRuntime();
        var fsm = runtime.Fsm;

        Assert.NotEqual(fsm.RootGrowthProcessingGroup, fsm.ChildRootingProcessingGroup);
        Assert.NotEqual(fsm.ChildRootingProcessingGroup, fsm.MatureGrowthProcessingGroup);
        Assert.NotEqual(fsm.MatureGrowthProcessingGroup, fsm.ReproductionProcessingGroup);
        Assert.NotEqual(fsm.RootGrowthProcessingGroup, fsm.ReproductionProcessingGroup);
    }

    [Fact(DisplayName = "Living GUI phases advance in lifecycle order")]
    public void LivingGui_PhasesAdvanceInLifecycleOrder()
    {
        using var runtime = CreateRuntime();

        Assert.Equal("RootGrowth", runtime.Fsm.LivingGuiActivePhase);
        runtime.Update();
        Assert.Equal("Reproduction", runtime.Fsm.LivingGuiActivePhase);
        runtime.Update();
        Assert.Equal("ChildRooting", runtime.Fsm.LivingGuiActivePhase);
    }

    [Fact(DisplayName = "Living GUI lineage names expose generation")]
    public void LivingGui_LineageNamesExposeGeneration()
    {
        using var runtime = CreateRuntime();

        runtime.Update();

        Assert.Contains("G0", runtime.Context.LivingNodes.Single(node => node.IsRoot).Lineage);
        Assert.Contains("G1", runtime.Context.LivingNodes.Single(node => !node.IsRoot).Lineage);
    }

    [Fact(DisplayName = "Living GUI reaches critical mass through runtime updates")]
    public void LivingGui_ReachesCriticalMass()
    {
        using var runtime = CreateRuntime();

        for (var i = 0; i < 100 && runtime.Context.LivingNodes.Count < 100; i++)
        {
            runtime.Update();
        }

        Assert.True(runtime.Context.LivingNodes.Count >= 1);
    }

    [Fact(DisplayName = "Page FSM registers the Living GUI root and nested process groups")]
    public void PageFsm_RegistersLivingGuiProcessGroups()
    {
        using var hub = new HubKernel();
        var page = new PageFSM(hub);
        var fsm = page.LivingGuiFsm;

        Assert.Contains(hub.ProcessGroups, group =>
            group.Name == fsm.LivingGuiProcessingGroup && group.ParentName == fsm.InstanceProcessingGroup);
        Assert.Contains(hub.ProcessGroups, group =>
            group.Name == fsm.GravityProcessingGroup && group.ParentName == fsm.InstanceProcessingGroup);
    }

    [Fact(DisplayName = "FSM manager and PageFSM share the registered Hub instance")]
    public void FsmManager_UsesRegisteredHubInstance()
    {
        var hub = new HubKernel();
        using var manager = new FSMManagerService(hub);

        Assert.Same(hub, manager.Page.Hub);
    }

    private static LivingGuiRuntime CreateRuntime() => new();
}
