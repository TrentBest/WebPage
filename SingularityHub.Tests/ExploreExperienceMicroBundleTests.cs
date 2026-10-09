using TheSingularityWorkshop.Gui;
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
    public void ExploreExperience_ReusesTheAlreadyLoadedMonikerBundle()
    {
        using var first = new ExploreExperienceMicroBundle();
        using var second = new ExploreExperienceMicroBundle();

        Assert.Same(first.Moniker, second.Moniker);
        Assert.True(first.Moniker.IsReady);
        Assert.Equal(MonikerMicroBundle.DefaultText, first.Moniker.Text);
    }

    [Fact]
    public void Moniker_ProvidesSafeDefaultThroughFsmApi()
    {
        using var moniker = new MonikerMicroBundle();

        Assert.True(moniker.IsReady);
        Assert.Equal("Ready", moniker.Status);
        Assert.Equal(MonikerMicroBundle.DefaultText, moniker.Text);

        moniker.Present();
        moniker.Update();
        Assert.Equal("Presenting", moniker.Status);
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
    public void SpatialNavigation_ApproachUsesDefinedRoomEntrance()
    {
        using var navigation = new SpatialNavigationMicroBundle();
        var room = new SpatialRoom(
            "forge", "The Forge", "FSM WORKSHOP",
            73, 30,
            73, 46,
            73, 46,
            "", "FORGE", null);

        navigation.ApproachEntrance(room);

        Assert.Equal(room.EntranceX, navigation.X);
        Assert.Equal(room.EntranceY, navigation.Y);

        navigation.MoveToExit(room);
        Assert.Equal(room.ExitX, navigation.X);
        Assert.Equal(room.ExitY, navigation.Y);
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
    [Fact]
    public void ExploreExperience_ExposesNestedContentDomainsAndFocusedLeaves()
    {
        using var experience = new ExploreExperienceMicroBundle();

        Assert.Equal((ulong)ExploreContentMicroBundle.BundleId, (ulong)experience.Content.Id);
        Assert.Equal(6, experience.Content.Domains.Count);

        Assert.Equal("SPATIAL WORKSHOP", experience.Content.Spatial.Name);
        Assert.Equal("AEC", experience.Content.Aec.Name);
        Assert.Equal("SINGULARITY LABORATORY", experience.Content.Laboratory.Name);
        Assert.Equal("SINGULARITY CITY", experience.Content.City.Name);
        Assert.Equal("CREATION", experience.Content.Creation.Name);
        Assert.Equal("AI / LANGUAGE", experience.Content.Ai.Name);

        Assert.Contains(experience.Content.Aec.Leaves, leaf => leaf.Name == "BIM");
        Assert.Contains(experience.Content.Laboratory.Leaves, leaf => leaf.Name == "SECURITY");
        Assert.Contains(experience.Content.City.Leaves, leaf => leaf.Name == "TRANSIT");
        Assert.Contains(experience.Content.Creation.Leaves, leaf => leaf.Name == "FSM FORGE");
        Assert.Contains(experience.Content.Ai.Leaves, leaf => leaf.Name == "WORKSHOP AI TERMINAL");

        Assert.Equal(
            experience.Content.Spatial.Id,
            ((MicroBundleContext)experience.Content.Spatial.Leaves[0].Context).ParentId);

        Assert.All(
            experience.Content.Leaves,
            leaf => Assert.True(leaf.Id > experience.Content.Id));
    }

    [Fact]
    public void ExploreContent_UsesUniqueIdsAndExplicitParentGenerationLineage()
    {
        using var experience = new ExploreExperienceMicroBundle();
        var content = experience.Content;
        var allBundles = new List<MicroBundleContext>
        {
            (MicroBundleContext)content.Context
        };

        allBundles.AddRange(content.Domains.Select(domain => (MicroBundleContext)domain.Context));
        allBundles.AddRange(content.Leaves.Select(leaf => (MicroBundleContext)leaf.Context));

        Assert.Equal(allBundles.Count, allBundles.Select(bundle => bundle.Id).Distinct().Count());

        Assert.Equal(0, ((MicroBundleContext)content.Context).Generation);
        Assert.Equal(-1, ((MicroBundleContext)content.Context).ParentId);

        foreach (var domain in content.Domains)
        {
            var domainContext = (MicroBundleContext)domain.Context;
            Assert.Equal(content.Id, domainContext.ParentId);
            Assert.Equal(1, domainContext.Generation);

            foreach (var leaf in domain.Leaves)
            {
                var leafContext = (MicroBundleContext)leaf.Context;
                Assert.Equal(domain.Id, leafContext.ParentId);
                Assert.Equal(2, leafContext.Generation);
            }
        }
    }

}
