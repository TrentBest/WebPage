using System.Linq;
using TheSingularityWorkshop.Services;
using HubKernel = TheSingularityWorkshop.SingularityHub.SingularityHub;
using Xunit;

namespace SingularityHub.Tests;

public sealed class LivingGuiRuntimeTests
{
    [Fact]
    public void PageFsm_ContextStartsValid()
    {
        using var fsm = new PageFSM();
        Assert.True(fsm.Context.IsValid);
        Assert.True(fsm.IsValid);
        Assert.True(fsm.LivingGuiIsValid);
    }

    [Fact]
    public void LivingGuiCreatesRootAndThenIndependentChildren()
    {
        using var fsm = new PageFSM();
        fsm.RequestEnter();

        for (var ticks = 0; ticks < 3 && fsm.Context.LivingNodes.Count == 0; ticks++)
            fsm.Update();

        var root = Assert.Single(fsm.Context.LivingNodes);
        Assert.Equal(PageFSM.LivingGuiPopulating, fsm.CurrentState);
        Assert.Equal("G:0", root.Lineage);
        Assert.Equal(50d, root.Size);
        Assert.Equal(PageStateContext.LivingNodePhase.Initialization, root.Phase);

        for (var ticks = 0; ticks < 200 && fsm.Context.LivingNodes.Count < 2; ticks++)
            fsm.Update();

        Assert.True(fsm.Context.LivingNodes.Count >= 2);
        var child = fsm.Context.LivingNodes[1];
        Assert.Equal(10d, child.Size);
        Assert.Equal(50d, child.X);
        Assert.Equal(50d, child.Y);
        Assert.Equal(PageStateContext.LivingNodePhase.Traveling, child.Phase);
        Assert.InRange(child.TargetX, 10d, 90d);
        Assert.InRange(child.TargetY, 10d, 90d);
        Assert.NotEqual(0d, child.Rotation);
    }

    [Fact]
    public void PageHeartbeatAdvancesOrganismFsmIntoGrowthAndReproduction()
    {
        using var fsm = new PageFSM();
        fsm.RequestEnter();

        for (var ticks = 0; ticks < 100 && fsm.Context.LivingNodes.Count == 0; ticks++)
            fsm.Update();

        var root = Assert.Single(fsm.Context.LivingNodes);
        Assert.Equal("G:0", root.Lineage);

        for (var ticks = 0; ticks < 100; ticks++)
            fsm.Update();

        Assert.True(root.Size > 50d, "The root organism never entered its FSM-driven growth cycle.");
        Assert.True(root.OffspringCount > 0, "The root organism never entered its FSM-driven reproduction cycle.");
        Assert.True(
            fsm.Context.LivingNodes.Count > 1,
            "The page heartbeat did not produce an independently attached organism.");
    }

    [Fact]
    public void OrganismsDoNotShareALifecyclePhase()
    {
        using var fsm = new PageFSM();
        fsm.RequestEnter();

        for (var ticks = 0; ticks < 10_000 && fsm.Context.LivingNodes.Count < 4; ticks++)
            fsm.Update();

        Assert.True(fsm.Context.LivingNodes.Count >= 4);

        var phases = fsm.Context.LivingNodes
            .Select(node => node.Phase)
            .Distinct()
            .ToArray();

        Assert.True(phases.Length >= 2, "The population collapsed into one shared lifecycle phase.");
    }

    [Fact]
    public void EachOrganismGetsItsOwnFsmAndOwnProcessingGroup()
    {
        var context = new PageStateContext();
        context.BeginLivingGui();
        var hub = new HubKernel();
        hub.RegisterProcessGroup("TestParent");

        using var runtime = new LivingGuiFsm(hub, context, "TestParent");

        for (var ticks = 0; ticks < 10_000 && context.LivingNodes.Count < 5; ticks++)
            runtime.Update();

        Assert.True(runtime.OrganismCount >= 5);
        Assert.Equal(context.LivingNodes.Count, runtime.OrganismCount);

        var groups = context.LivingNodes
            .Select((_, index) => index)
            .Select(index => hub.ProcessGroups
                .Where(group => group.Name.Contains("LivingGui:Organism:"))
                .Select(group => group.Name)
                .Distinct()
                .ElementAt(index))
            .ToArray();

        Assert.Equal(groups.Length, groups.Distinct().Count());
    }

    [Fact]
    public void OrganismsAdvanceIndependentlyThroughTheirOwnFsmGroups()
    {
        var context = new PageStateContext();
        context.BeginLivingGui();
        var hub = new HubKernel();
        hub.RegisterProcessGroup("TestParent");

        using var runtime = new LivingGuiFsm(hub, context, "TestParent");

        for (var ticks = 0; ticks < 2_000 && context.LivingNodes.Count < 3; ticks++)
            runtime.Update();

        var children = context.LivingNodes.Where(node => !node.IsRoot).Take(2).ToArray();
        Assert.Equal(2, children.Length);

        for (var ticks = 0; ticks < 5; ticks++)
            runtime.Update();

        Assert.True(
            children[0].X != children[1].X ||
            children[0].Y != children[1].Y ||
            children[0].Rotation != children[1].Rotation,
            "Independent organism FSMs collapsed into identical kinematics.");
    }

    [Fact]
    public void LivingGuiUsesLerpedMovementRatherThanFrameSizedJumps()
    {
        var context = new PageStateContext();
        context.BeginLivingGui();
        var hub = new HubKernel();
        hub.RegisterProcessGroup("TestParent");

        using var runtime = new LivingGuiFsm(hub, context, "TestParent");

        for (var ticks = 0; ticks < 100 && context.LivingNodes.Count < 2; ticks++)
            runtime.Update();

        var child = context.LivingNodes[1];
        var previousDistance = double.MaxValue;
        var observedMovement = false;

        for (var ticks = 0; ticks < 20 && child.Phase == PageStateContext.LivingNodePhase.Traveling; ticks++)
        {
            var distance = System.Math.Sqrt(
                System.Math.Pow(child.TargetX - child.X, 2) +
                System.Math.Pow(child.TargetY - child.Y, 2));

            if (previousDistance < double.MaxValue)
                Assert.True(distance < previousDistance);

            previousDistance = distance;
            runtime.Update();
            observedMovement = true;
        }

        Assert.True(observedMovement);
    }

    [Fact]
    public void LivingGuiPublishesPopulationChangeAfterFsmAdvance()
    {
        var context = new PageStateContext();
        context.BeginLivingGui();
        var hub = new HubKernel();
        hub.RegisterProcessGroup("TestParent");

        using var runtime = new LivingGuiFsm(hub, context, "TestParent");
        var notifications = 0;
        runtime.PopulationChanged += () => notifications++;

        runtime.Update();

        Assert.Equal(1, notifications);
        Assert.Equal(50d, context.LivingNodes[0].Size);
        Assert.Equal(PageStateContext.LivingNodePhase.Existing, context.LivingNodes[0].Phase);
    }

    [Fact]
    public void PageFSMRegistersLivingGuiAndGravityGroups()
    {
        using var fsm = new PageFSM();

        Assert.Contains(fsm.Hub.ProcessGroups, group =>
            group.Name == fsm.InstanceProcessingGroup && group.ParentName is null);
        Assert.Contains(fsm.Hub.ProcessGroups, group =>
            group.Name == fsm.LivingGuiProcessingGroup && group.ParentName == fsm.InstanceProcessingGroup);
        Assert.Contains(fsm.Hub.ProcessGroups, group =>
            group.Name == fsm.GravityProcessingGroup && group.ParentName == fsm.InstanceProcessingGroup);
    }

    [Fact]
    public void FsmManagerUsesRegisteredHubInstance()
    {
        var hub = new HubKernel();
        using var manager = new FSMManagerService(hub);
        Assert.Same(hub, manager.Page.Hub);
    }
}
