using TheSingularityWorkshop.Workshop.MicroBundles;

namespace SingularityHub.Tests;

public sealed class ExploreExperienceMicroBundleTests
{
    [Fact]
    public void ExploreExperience_ComposesSpatialConferenceAndForgeCapabilities()
    {
        using var experience = new ExploreExperienceMicroBundle();

        Assert.Equal((ulong)ExploreExperienceMicroBundle.BundleId, experience.Id);
        Assert.Equal((ulong)SpatialNavigationMicroBundle.BundleId, experience.Navigation.Id);
        Assert.Equal((ulong)ConferenceMicroBundle.BundleId, experience.Conference.Id);
        Assert.Equal((ulong)FsmForgeMicroBundle.BundleId, experience.Forge.Id);
    }

    [Fact]
    public void SpatialNavigation_ClampsAndMovesAuthoritatively()
    {
        using var navigation = new SpatialNavigationMicroBundle(50, 54);

        navigation.MoveTo(0, 100);
        Assert.Equal(4, navigation.X);
        Assert.Equal(92, navigation.Y);

        navigation.Move(10, -10);
        Assert.Equal(14, navigation.X);
        Assert.Equal(82, navigation.Y);
    }

    [Fact]
    public void SpatialNavigation_ApproachProducesRoomEntryPosition()
    {
        using var navigation = new SpatialNavigationMicroBundle();

        navigation.Approach(73, 30);

        Assert.Equal(73, navigation.X);
        Assert.Equal(36, navigation.Y);
    }

    [Fact]
    public void ConferenceMembership_IsOwnedByConferenceMicroBundle()
    {
        using var conference = new ConferenceMicroBundle();

        Assert.False(conference.Joined);
        Assert.Equal(0, conference.ParticipantCount);

        conference.Join();
        Assert.True(conference.Joined);
        Assert.Equal(1, conference.ParticipantCount);

        conference.Leave();
        Assert.False(conference.Joined);
        Assert.Equal(0, conference.ParticipantCount);
    }
}
