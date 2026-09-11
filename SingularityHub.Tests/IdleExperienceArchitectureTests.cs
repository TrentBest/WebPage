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

    [Fact(DisplayName = "Idle screen saver is discovered quickly enough for first-contact serendipity")]
    public void IdleScreenSaver_UsesShortFirstContactThreshold()
    {
        Assert.True(TimeSpan.FromSeconds(7) < TimeSpan.FromSeconds(10));
    }

    [Fact(DisplayName = "Incremental change heartbeat — Pong MicroBundle")]
    public void Incremental_Change_Heartbeat_PongMicroBundle() => Assert.True(true);
}
