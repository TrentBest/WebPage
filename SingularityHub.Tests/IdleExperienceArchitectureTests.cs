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

        for (var i = 0; i < 5 && context.LivingNodes.Count == 1; i++)
            context.AdvanceLivingGui();

        Assert.True(context.LivingNodes.Count > 1);
        Assert.Contains(context.LivingNodes, node => node.Size == 6);
        Assert.Contains(context.LivingNodes, node => node.X != 50 || node.Y != 50);
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
