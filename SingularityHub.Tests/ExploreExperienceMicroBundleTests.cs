using TheSingularityWorkshop.Gui;
using TheSingularityWorkshop.Workshop.Experience;
using TheSingularityWorkshop.Workshop.MicroBundles;
using Xunit;

namespace SingularityHub.Tests;

public sealed class ExploreExperienceMicroBundleTests
{
    [Fact]
    public void ExploreExperience_ComposesSpatialUserConferenceAndForgeCapabilities()
    {
        using var experience = new ExploreExperienceMicroBundle();

        Assert.Equal((ulong)ExploreExperienceMicroBundle.BundleId, experience.Id);
        Assert.Equal((ulong)SpatialNavigationMicroBundle.BundleId, experience.Navigation.Id);
        Assert.Equal((ulong)UserMicroBundle.BundleId, experience.User.Id);
        Assert.Equal((ulong)ConferenceMicroBundle.BundleId, experience.Conference.Id);
        Assert.Equal((ulong)FsmForgeMicroBundle.BundleId, experience.Forge.Id);
    }

    [Fact]
    public void SpatialNavigation_DefaultsVisitorToWorldCenter()
    {
        using var navigation = new SpatialNavigationMicroBundle();

        Assert.Equal(SpatialNavigationMicroBundle.WorldCenterX, navigation.X);
        Assert.Equal(SpatialNavigationMicroBundle.WorldCenterY, navigation.Y);
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
    public void SpatialNavigation_ApproachUsesDefinedExperienceEntrance()
    {
        using var navigation = new SpatialNavigationMicroBundle();
        var experienceSpace = new ExperienceSpace(
            "forge", "The Forge", "FSM WORKSHOP",
            73, 30,
            73, 46,
            73, 46,
            "", "FORGE", null);

        navigation.ApproachEntrance(experienceSpace);

        Assert.Equal(experienceSpace.EntranceX, navigation.X);
        Assert.Equal(experienceSpace.EntranceY, navigation.Y);

        navigation.MoveToExit(experienceSpace);
        Assert.Equal(experienceSpace.ExitX, navigation.X);
        Assert.Equal(experienceSpace.ExitY, navigation.Y);
    }

    [Fact]
    public void User_ProvidesAnonymousIdentityAndAccountPromotion()
    {
        using var user = new UserMicroBundle();

        Assert.True(user.IsAnonymous);
        Assert.False(user.IsAuthenticated);
        Assert.Equal("Anonymous", user.DisplayName);

        user.CreateAccount("Workshop Visitor");

        Assert.False(user.IsAnonymous);
        Assert.True(user.IsAuthenticated);
        Assert.Equal("Workshop Visitor", user.DisplayName);
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

    [Fact]
    public void FsmForge_ExpressesItsOwnPresentationAndFacadeState()
    {
        using var forge = new FsmForgeMicroBundle();

        Assert.Equal("The Forge", forge.Presentation.Title);
        Assert.Equal("FSM WORKSHOP", forge.Presentation.Surface);
        Assert.Contains("STATE LIFECYCLE", forge.Presentation.Capabilities);
        Assert.Equal("NOT STARTED", forge.State);

        forge.Start();

        Assert.True(forge.IsRunning);
    }

    [Fact]
    public void SoftwarePatterns_FormFocusedBundlesGroupsAndAnnotatedHierarchy()
    {
        using var patterns = new SoftwarePatternsMicroBundle();
        using var annotation = new SoftwarePatternsAnnotationMicroBundle(patterns);

        Assert.Equal((ulong)SoftwarePatternsMicroBundle.BundleId, patterns.Id);
        Assert.Equal(3, patterns.Groups.Count);
        Assert.Equal(8, patterns.Patterns.Count);

        Assert.Equal((ulong)CreationalPatternsMicroBundle.BundleId, patterns.Creational.Id);
        Assert.Equal((ulong)StructuralPatternsMicroBundle.BundleId, patterns.Structural.Id);
        Assert.Equal((ulong)BehavioralPatternsMicroBundle.BundleId, patterns.Behavioral.Id);

        Assert.Contains(patterns.Creational.Patterns, pattern => pattern is FactoryMicroBundle);
        Assert.Contains(patterns.Creational.Patterns, pattern => pattern is BuilderMicroBundle);
        Assert.Contains(patterns.Structural.Patterns, pattern => pattern is FacadeMicroBundle);
        Assert.Contains(patterns.Structural.Patterns, pattern => pattern is CompositeMicroBundle);
        Assert.Contains(patterns.Behavioral.Patterns, pattern => pattern is CommandMicroBundle);
        Assert.Contains(patterns.Behavioral.Patterns, pattern => pattern is ObserverMicroBundle);
        Assert.Contains(patterns.Behavioral.Patterns, pattern => pattern is StrategyMicroBundle);

        Assert.Equal(12, annotation.ReferencedBundles.Count);
        Assert.Equal(12, annotation.Annotations.Count);
        Assert.All(annotation.Annotations, item => Assert.NotEmpty(item.Tags));
    }

    [Fact]
    public void ExploreExperience_ExposesPatternFamilyAndAnnotationAsMicroBundles()
    {
        using var experience = new ExploreExperienceMicroBundle();

        Assert.Equal((ulong)SoftwarePatternsMicroBundle.BundleId, experience.SoftwarePatterns.Id);
        Assert.Equal((ulong)SoftwarePatternsAnnotationMicroBundle.BundleId, experience.SoftwarePatternsAnnotation.Id);
        Assert.Same(experience.SoftwarePatterns, experience.SoftwarePatternsAnnotation.Patterns);
        Assert.Equal(8, experience.SoftwarePatterns.Patterns.Count);
    }
}
