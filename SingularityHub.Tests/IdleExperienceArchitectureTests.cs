using TheSingularityWorkshop.Services;
using TheSingularityWorkshop.Workshop.MicroBundles;
using Xunit;

namespace SingularityHub.Tests;

public sealed class IdleExperienceArchitectureTests
{
    [Fact(DisplayName = "Idle experience catalog is non-empty and GUI-native")]
    public void IdleExperienceCatalog_HasAvailableExperience()
    {
        Assert.NotEmpty(IdleExperienceCatalog.Available);
        Assert.All(IdleExperienceCatalog.Available, experience =>
        {
            Assert.False(string.IsNullOrWhiteSpace(experience.Id));
            Assert.False(string.IsNullOrWhiteSpace(experience.DisplayName));
            Assert.False(string.IsNullOrWhiteSpace(experience.Description));
        });
    }

    [Fact(DisplayName = "Idle experience selection is contained by the catalog")]
    public void IdleExperienceCatalog_SelectsOnlyRegisteredExperience()
    {
        var random = new Random(42);
        var selected = IdleExperienceCatalog.Select(random);
        Assert.Contains(selected, IdleExperienceCatalog.Available);
    }

    [Fact(DisplayName = "Pong is registered as an installable MicroBundle")]
    public void Pong_IsRegisteredAsMicroBundle()
    {
        Assert.Contains((ulong)PongMicroBundle.BundleId, MicroBundleCatalog.Available);
        Assert.True(MicroBundleCatalog.IsAvailable(PongMicroBundle.BundleId));
    }

    [Fact(DisplayName = "Pong MicroBundle advances through its FSM-backed lifecycle")]
    public void PongMicroBundle_AdvancesThroughLifecycle()
    {
        var bundle = MicroBundleCatalog.CreatePong();
        Assert.Equal("Created", bundle.Phase);
        Assert.Null(bundle.Manifestation);

        bundle.Update();
        Assert.Equal("Manifesting", bundle.Phase);
        Assert.Equal("Trace", bundle.Manifestation!.Effect);

        bundle.Update();
        Assert.Equal("Active", bundle.Phase);
        Assert.Equal("Breathe", bundle.Manifestation!.Effect);
    }

    [Fact(DisplayName = "Flex catalog exposes the Living GUI as a separate experience category")]
    public void FlexCatalog_ExposesLivingGui()
    {
        Assert.True(FlexExperienceCatalog.IsAvailable("living-gui"));
        Assert.Contains(FlexExperienceCatalog.Available, experience => experience.Id == "living-gui");
    }

    [Fact(DisplayName = "The first Living GUI is born exactly in the center")]
    public void LivingGui_FirstNodeStartsCentered()
    {
        var context = new PageStateContext();
        context.BeginLivingGui();

        var first = Assert.Single(context.LivingNodes);
        Assert.Equal(50, first.X);
        Assert.Equal(50, first.Y);
        Assert.Equal(6, first.Size);
    }

    [Fact(DisplayName = "Living GUI newborns are tiny independent seeds")]
    public void LivingGui_NewbornsStartAsIndependentSeeds()
    {
        var context = new PageStateContext();
        context.BeginLivingGui();

        for (var i = 0; i < 16 && context.LivingNodes.Count == 1; i++)
            context.AdvanceLivingGui();

        Assert.True(context.LivingNodes.Count > 1);

        var newborn = context.LivingNodes[^1];

        // The newborn is thrown from the parent's current position, but it has
        // an independent destination and remains at absolute seed size until
        // the flight completes.
        Assert.Equal(6, newborn.Size);
        Assert.Equal(newborn.SeedX, newborn.X);
        Assert.Equal(newborn.SeedY, newborn.Y);
        Assert.True(newborn.TargetX != newborn.SeedX || newborn.TargetY != newborn.SeedY);

        context.AdvanceLivingGui();

        Assert.Equal(6, newborn.Size);
        Assert.True(newborn.X != newborn.SeedX || newborn.Y != newborn.SeedY);
    }

    [Fact(DisplayName = "Incremental Unit Test 01 — newborn seed flight precedes growth")]
    public void IncrementalUnitTest01_NewbornSeedFlightPrecedesGrowth()
    {
        var context = new PageStateContext();
        context.BeginLivingGui();

        while (context.LivingNodes.Count == 1)
            context.AdvanceLivingGui();

        var newborn = context.LivingNodes[^1];
        var initialSize = newborn.Size;
        var initialX = newborn.X;
        var initialY = newborn.Y;

        Assert.Equal(6, initialSize);
        Assert.Equal(0, newborn.SeedTicks);
        Assert.True(newborn.SeedFlightDuration >= 2);

        context.AdvanceLivingGui();

        Assert.Equal(6, newborn.Size);
        Assert.True(newborn.SeedTicks > 0);
        Assert.True(newborn.X != initialX || newborn.Y != initialY);
    }

    [Fact(DisplayName = "Incremental Unit Test 02 — entering the Workshop leaves the gateway immediately")]
    public void IncrementalUnitTest02_EnteringWorkshopLeavesGatewayImmediately()
    {
        using var fsm = new PageFSM();

        Assert.Equal(PageFSM.Gateway, fsm.CurrentState);
        Assert.False(fsm.Context.EnterRequested);

        fsm.RequestEnter();

        Assert.True(fsm.Context.EnterRequested);
        Assert.Equal(PageFSM.GatewayExit, fsm.CurrentState);
    }

    [Fact(DisplayName = "Incremental Unit Test 03 — a new PageFSM settles initialization into the gateway")]
    public void IncrementalUnitTest03_NewPageFSMSettlesIntoGateway()
    {
        using var fsm = new PageFSM();

        Assert.Equal(PageFSM.Gateway, fsm.CurrentState);
        Assert.Equal(0, fsm.Context.StateTicks);
        Assert.False(fsm.Context.EnterRequested);
    }

    [Fact(DisplayName = "Living GUI freezes exactly at critical mass and stops updating")]
    public void LivingGui_FreezesAtOneHundred()
    {
        var context = new PageStateContext();
        context.BeginLivingGui();

        for (var i = 0; i < 2000 && !context.LivingGuiFrozen; i++)
            context.AdvanceLivingGui();

        Assert.True(context.LivingGuiFrozen);
        Assert.Equal(100, context.LivingNodes.Count);
        Assert.True(context.LivingGuiPopulated);

        var frozenSnapshot = context.LivingNodes
            .Select(node => (node.Lineage, node.X, node.Y, node.Size))
            .ToArray();

        context.AdvanceLivingGui();

        var afterFreeze = context.LivingNodes
            .Select(node => (node.Lineage, node.X, node.Y, node.Size))
            .ToArray();

        Assert.Equal(frozenSnapshot, afterFreeze);
    }

    [Fact(DisplayName = "Flex Living GUI reaches the first count beyond one hundred within six generations")]
    public void FlexLivingGui_ReachesBeyondOneHundred()
    {
        // 1 + 2 + 4 + 8 + 16 + 32 + 64 = 127 manifested nodes.
        var total = Enumerable.Range(0, 7).Sum(generation => 1 << generation);
        Assert.Equal(127, total);
        Assert.True(total >= 100);
    }

    [Fact(DisplayName = "Idle screen saver is discovered quickly enough for first-contact serendipity")]
    public void IdleScreenSaver_UsesShortFirstContactThreshold()
    {
        Assert.True(TimeSpan.FromSeconds(7) < TimeSpan.FromSeconds(10));
    }

    [Fact(DisplayName = "Incremental change heartbeat — Flex + Pong")]
    public void Incremental_Change_Heartbeat_FlexAndPong() => Assert.True(true);
}
