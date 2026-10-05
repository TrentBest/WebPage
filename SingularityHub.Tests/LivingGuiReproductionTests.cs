using TheSingularityWorkshop.Services;
using Xunit;

namespace SingularityHub.Tests;

public sealed class LivingGuiReproductionTests
{
    [Fact(DisplayName = "Each organism completes its own flight, planting, growth, reproduction, and reduction cycle")]
    public void IncrementalUnitTest60_OrganismLifecycleIsIndependent()
    {
        var context = new PageStateContext();
        context.BeginLivingGui();

        var root = context.LivingNodes[0];
        var hub = new TheSingularityWorkshop.SingularityHub.SingularityHub();
        hub.RegisterProcessGroup("TestParent");

        using var runtime = new LivingGuiFsm(hub, context, "TestParent");

        for (var i = 0; i < 100 && context.LivingNodes.Count < 2; i++)
            runtime.Update();

        var child = Assert.Single(context.LivingNodes, node => !node.IsRoot);
        Assert.Equal(1, child.Generation);
        Assert.Equal(10d, child.Size);
        Assert.Equal(PageStateContext.LivingNodePhase.Traveling, child.Phase);

        var guard = 2_000;
        while (child.Phase != PageStateContext.LivingNodePhase.Existing && guard-- > 0)
            runtime.Update();

        Assert.True(guard > 0);
        Assert.Equal(50d, child.Size);
        Assert.Equal(PageStateContext.LivingNodePhase.Existing, child.Phase);

        guard = 2_000;
        while (child.Phase != PageStateContext.LivingNodePhase.Reproducing && guard-- > 0)
            runtime.Update();

        Assert.True(guard > 0);
        Assert.Equal(100d, child.Size);

        var populationBefore = context.LivingNodes.Count;
        runtime.Update();

        Assert.True(context.LivingNodes.Count > populationBefore);
        Assert.Equal(PageStateContext.LivingNodePhase.Reducing, child.Phase);

        guard = 100;
        while (child.Phase == PageStateContext.LivingNodePhase.Reducing && guard-- > 0)
            runtime.Update();

        Assert.True(guard > 0);
        Assert.Equal(50d, child.Size);
        Assert.Equal(PageStateContext.LivingNodePhase.Existing, child.Phase);
        Assert.NotSame(child, context.LivingNodes[0]);
        Assert.NotEqual(child.Lineage, root.Lineage);

        // The same organism must be able to repeat the complete life cycle:
        // maximum size -> immediate reproduction -> reduction -> default size -> growth.
        var offspringBeforeSecondPeak = child.OffspringCount;
        guard = 2_000;
        while (child.Phase != PageStateContext.LivingNodePhase.Reproducing && guard-- > 0)
            runtime.Update();

        Assert.True(guard > 0);
        Assert.Equal(100d, child.Size);

        runtime.Update();

        Assert.True(child.OffspringCount > offspringBeforeSecondPeak);
        Assert.Equal(PageStateContext.LivingNodePhase.Reducing, child.Phase);
    }

    [Fact(DisplayName = "Reproduction attaches a new FSM without moving the existing parent lifecycle")]
    public void IncrementalUnitTest62_ReproductionAttachesIndependentChild()
    {
        var context = new PageStateContext();
        context.BeginLivingGui();
        var hub = new TheSingularityWorkshop.SingularityHub.SingularityHub();
        hub.RegisterProcessGroup("TestParent");

        using var runtime = new LivingGuiFsm(hub, context, "TestParent");

        for (var i = 0; i < 100 && context.LivingNodes.Count < 3; i++)
            runtime.Update();

        Assert.True(context.LivingNodes.Count >= 3);
        Assert.True(runtime.OrganismCount >= 3);
        Assert.All(context.LivingNodes, node => Assert.NotEqual(PageStateContext.LivingNodePhase.Initialization, node.Phase));
    }
}
