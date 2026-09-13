using TheSingularityWorkshop.Services;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>
/// Behavioral contract for the Living GUI's generational reproduction cycle.
/// This test deliberately exercises the context directly so the lifecycle can be
/// verified without coupling the assertion to Razor timing or browser rendering.
/// </summary>
public sealed class LivingGuiReproductionTests
{
    [Fact(DisplayName = "Incremental Unit Test 60 — a GUI seed flies, scales, grows, reproduces, and its parent recovers")]
    public void IncrementalUnitTest60_SeedLifecycleIsGenerational()
    {
        var context = new PageStateContext();
        context.BeginLivingGui();

        var root = context.LivingNodes[0];
        Assert.Equal(PageStateContext.LivingNodePhase.RootGrowth, root.Phase);
        Assert.Equal(200, root.Size);

        for (var i = 0; i < 4; i++)
            context.AdvanceRootGrowth();

        Assert.Equal(400, root.Size);
        Assert.Equal(PageStateContext.LivingNodePhase.ReproductionPending, root.Phase);

        context.AdvanceReproduction();

        Assert.Equal(2, context.LivingNodes.Count);
        Assert.Equal(400, root.Size);
        Assert.Equal(PageStateContext.LivingNodePhase.ParentRecovery, root.Phase);

        var seed = context.LivingNodes[1];
        Assert.Equal(PageStateContext.LivingNodePhase.SeedFlight, seed.Phase);
        Assert.Equal(40, seed.Size);
        Assert.Equal(1, seed.Generation);
        Assert.Equal(root.X, seed.X);
        Assert.Equal(root.Y, seed.Y);
        Assert.InRange(seed.TargetX, 10, 90);
        Assert.InRange(seed.TargetY, 10, 90);

        var launchX = seed.X;
        var launchY = seed.Y;
        context.AdvanceSeedFlight();
        Assert.True(seed.Phase is PageStateContext.LivingNodePhase.SeedFlight or PageStateContext.LivingNodePhase.SeedScaling);
        Assert.True(seed.X != launchX || seed.Y != launchY || seed.X == seed.TargetX && seed.Y == seed.TargetY);

        for (var i = 0; i < 100 && seed.Phase == PageStateContext.LivingNodePhase.SeedFlight; i++)
            context.AdvanceSeedFlight();

        Assert.Equal(PageStateContext.LivingNodePhase.SeedScaling, seed.Phase);
        Assert.Equal(seed.TargetX, seed.X);
        Assert.Equal(seed.TargetY, seed.Y);
        Assert.Equal(40, seed.Size);

        for (var i = 0; i < 10 && seed.Phase == PageStateContext.LivingNodePhase.SeedScaling; i++)
            context.AdvanceSeedScaling();

        Assert.Equal(PageStateContext.LivingNodePhase.MatureGrowth, seed.Phase);
        Assert.Equal(200, seed.Size);
        Assert.True(seed.GrowthReady);

        for (var i = 0; i < 10 && seed.Phase == PageStateContext.LivingNodePhase.MatureGrowth; i++)
            context.AdvanceMatureGrowth();

        Assert.Equal(400, seed.Size);
        Assert.Equal(PageStateContext.LivingNodePhase.ReproductionPending, seed.Phase);

        context.AdvanceReproduction();

        Assert.Equal(3, context.LivingNodes.Count);
        Assert.Equal(400, seed.Size);
        Assert.Equal(PageStateContext.LivingNodePhase.ParentRecovery, seed.Phase);
        Assert.Equal(PageStateContext.LivingNodePhase.SeedFlight, context.LivingNodes[2].Phase);

        context.AdvanceParentRecovery();
        Assert.Equal(300, root.Size);
        Assert.Equal(300, seed.Size);
        Assert.Equal(PageStateContext.LivingNodePhase.ParentRecovery, root.Phase);
        Assert.Equal(PageStateContext.LivingNodePhase.ParentRecovery, seed.Phase);

        context.AdvanceParentRecovery();
        Assert.Equal(200, root.Size);
        Assert.Equal(200, seed.Size);
        Assert.Equal(PageStateContext.LivingNodePhase.MatureGrowth, root.Phase);
        Assert.Equal(PageStateContext.LivingNodePhase.MatureGrowth, seed.Phase);
    }

    [Fact(DisplayName = "Incremental Unit Test 62 — reproduction parents recover before newborn flight")]
    public void IncrementalUnitTest62_ReproductionParentRecoversBeforeSeedFlight()
    {
        var context = new PageStateContext();
        context.BeginLivingGui();
        for (var i = 0; i < 4; i++)
            context.AdvanceRootGrowth();
        context.AdvanceReproduction();

        var parent = context.LivingNodes[0];
        var seed = context.LivingNodes[1];

        Assert.Equal(PageStateContext.LivingNodePhase.ParentRecovery, parent.Phase);
        Assert.Equal(PageStateContext.LivingNodePhase.SeedFlight, seed.Phase);

        var hub = new TheSingularityWorkshop.SingularityHub.SingularityHub();
        hub.RegisterProcessGroup("TestParent");

        using var runtime = new LivingGuiFsm(hub, context, "TestParent");

        Assert.Equal(LivingGuiFsm.ParentRecoveryState, runtime.ActivePhase);
        runtime.Update();

        Assert.Equal(300, parent.Size);
        Assert.Equal(PageStateContext.LivingNodePhase.ParentRecovery, parent.Phase);
        Assert.Equal(PageStateContext.LivingNodePhase.SeedFlight, seed.Phase);
        Assert.Equal(LivingGuiFsm.ParentRecoveryState, runtime.ActivePhase);
    }
}