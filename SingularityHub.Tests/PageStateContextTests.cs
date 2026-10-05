using System;
using TheSingularityWorkshop.Services;
using Xunit;

namespace SingularityHub.Tests;

public sealed class PageStateContextTests
{
    [Fact] public void ConstructorStartsValidAndEmpty()
    {
        var context = new PageStateContext();
        Assert.True(context.IsValid);
        Assert.Equal("PageFSMContext", context.Name);
        Assert.Empty(context.LivingNodes);
    }

    [Fact] public void BeginLivingGuiCreatesOneGenerationZeroRoot()
    {
        var context = new PageStateContext();
        context.BeginLivingGui();
        var root = Assert.Single(context.LivingNodes);

        Assert.Equal("G:0", root.Lineage);
        Assert.Equal(0, root.Generation);
        Assert.True(root.IsRoot);
        Assert.Equal(50d, root.X);
        Assert.Equal(50d, root.Y);
        Assert.Equal(50d, root.Size);
        Assert.Equal(PageStateContext.LivingNodePhase.Initialization, root.Phase);
    }

    [Fact] public void RootInitializationTransitionsDirectlyToExisting()
    {
        var context = new PageStateContext();
        context.BeginLivingGui();
        var root = context.LivingNodes[0];

        context.InitializeOrganism(root);

        Assert.Equal(PageStateContext.LivingNodePhase.Existing, root.Phase);
        Assert.Equal(50d, root.X);
        Assert.Equal(50d, root.Y);
    }

    [Fact] public void OffspringStartsAtParentAndReceivesIndependentTargetDuringInitialization()
    {
        var context = new PageStateContext();
        context.BeginLivingGui();
        var root = context.LivingNodes[0];

        var child = context.CreateOffspring(root);
        context.InitializeOrganism(child);

        Assert.Equal(2, context.LivingNodes.Count);
        Assert.Equal(50d, child.X);
        Assert.Equal(50d, child.Y);
        Assert.Equal(10d, child.Size);
        Assert.Equal(PageStateContext.LivingNodePhase.Traveling, child.Phase);
        Assert.InRange(child.TargetX, 10d, 90d);
        Assert.InRange(child.TargetY, 10d, 90d);
    }

    [Fact] public void TravelMovesTowardTargetWithoutJumping()
    {
        var context = new PageStateContext();
        context.BeginLivingGui();
        var child = context.CreateOffspring(context.LivingNodes[0]);
        context.InitializeOrganism(child);

        var before = Distance(child.X, child.Y, child.TargetX, child.TargetY);
        context.AdvanceTravel(child);
        var after = Distance(child.X, child.Y, child.TargetX, child.TargetY);

        Assert.True(after < before);
        Assert.Equal(PageStateContext.LivingNodePhase.Traveling, child.Phase);
    }

    [Fact] public void PlantingLerpsToDefaultSize()
    {
        var context = new PageStateContext();
        context.BeginLivingGui();
        var child = context.CreateOffspring(context.LivingNodes[0]);
        context.InitializeOrganism(child);
        child.X = child.TargetX;
        child.Y = child.TargetY;
        context.AdvanceTravel(child);

        var guard = 100;
        while (child.Phase == PageStateContext.LivingNodePhase.Planting && guard-- > 0)
            context.AdvancePlanting(child);

        Assert.Equal(PageStateContext.LivingNodePhase.Existing, child.Phase);
        Assert.Equal(50d, child.Size);
    }

    [Fact] public void ExistingLerpsToDoubleAndThenReproduces()
    {
        var context = new PageStateContext();
        context.BeginLivingGui();
        var root = context.LivingNodes[0];
        context.InitializeOrganism(root);

        var guard = 100;
        while (root.Phase == PageStateContext.LivingNodePhase.Existing && guard-- > 0)
            context.AdvanceExisting(root);

        Assert.Equal(100d, root.Size);
        Assert.Equal(PageStateContext.LivingNodePhase.Reproducing, root.Phase);
    }

    [Fact] public void PopulationCanReachThresholdThroughIndependentOrganismRuntime()
    {
        var context = new PageStateContext();
        context.BeginLivingGui();
        var hub = new TheSingularityWorkshop.SingularityHub.SingularityHub();
        hub.RegisterProcessGroup("TestParent");

        using var runtime = new LivingGuiFsm(hub, context, "TestParent");

        const int guard = 10_000;
        var ticks = 0;
        while (context.LivingNodes.Count < PageStateContext.PopulationObservationThreshold && ticks++ < guard)
            runtime.Update();

        Assert.Equal(PageStateContext.PopulationObservationThreshold, context.LivingNodes.Count);
        Assert.Contains(context.LivingNodes, node => node.Generation > 1);
        Assert.False(context.LivingGuiFrozen);
    }

    [Fact] public void ResetStateClockClearsOnlyStateTicks()
    {
        var context = new PageStateContext { StateTicks = 37, TotalTicks = 91 };
        context.ResetStateClock();
        Assert.Equal(0, context.StateTicks);
        Assert.Equal(91, context.TotalTicks);
    }

    private static double Distance(double x1, double y1, double x2, double y2)
        => Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
}
