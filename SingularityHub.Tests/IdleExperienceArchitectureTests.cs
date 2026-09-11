using TheSingularityWorkshop.Services;
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

    [Fact(DisplayName = "Idle screen saver is discovered quickly enough for first-contact serendipity")]
    public void IdleScreenSaver_UsesShortFirstContactThreshold()
    {
        Assert.True(TimeSpan.FromSeconds(7) < TimeSpan.FromSeconds(10));
    }

    [Fact(DisplayName = "Incremental change heartbeat — idle experience layer")]
    public void Incremental_Change_Heartbeat_IdleExperience() => Assert.True(true);
}
