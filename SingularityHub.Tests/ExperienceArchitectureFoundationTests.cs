using TheSingularityWorkshop.SingularityHub;
using TheSingularityWorkshop.Workshop.MicroBundles;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>
/// Architectural guardrails for the Workshop's Experience/MicroBundle boundary.
/// These tests prevent identity collisions and ensure the spatial composition is
/// represented by the Hub-level Experience contract rather than only by a page.
/// </summary>
public sealed class ExperienceArchitectureFoundationTests : IDisposable
{
    public ExperienceArchitectureFoundationTests() => TheSingularityWorkshop.FSM_API.FSM_API.Internal.ResetAPI(true);

    [Fact]
    public void ExploreExperience_UsesUniqueMicroBundleIdentities()
    {
        using var experience = new ExploreExperienceMicroBundle();

        var ids = new[]
        {
            experience.Navigation.Id,
            experience.Pathfinding.Id,
            experience.User.Id,
            experience.Conference.Id,
            experience.Forge.Id,
            experience.SoftwarePatterns.Id,
            experience.SoftwarePatternsAnnotation.Id
        };

        Assert.Equal(ids.Length, ids.Distinct().Count());
        Assert.Equal((ulong)PathfindingMicroBundle.BundleId, experience.Pathfinding.Id);
        Assert.Equal((ulong)ConferenceMicroBundle.BundleId, experience.Conference.Id);
        Assert.NotEqual(experience.User.Id, experience.Conference.Id);
    }

    [Fact]
    public void ExploreExperience_ImplementsHubExperienceComposition()
    {
        using var concrete = new ExploreExperienceMicroBundle();
        var experience = (IExperience)concrete;

        Assert.Equal(ExploreExperienceMicroBundle.ExperienceId, experience.Id);
        Assert.Equal("WORKSHOP EXPLORE", experience.Name);
        Assert.Contains(concrete.Navigation.Id, experience.MicroBundleIds);
        Assert.Contains(concrete.Pathfinding.Id, experience.MicroBundleIds);
        Assert.Contains(concrete.Forge.Id, experience.MicroBundleIds);
        Assert.Contains(HubCapabilityIds.SpatialNavigation, experience.Capabilities);
        Assert.Contains(HubCapabilityIds.Pathfinding, experience.Capabilities);
        Assert.Contains(HubCapabilityIds.FsmForge, experience.Capabilities);
        Assert.Equal(1, experience.SenseCount);
        Assert.Contains("MicroBundle_2105", experience.ProcessingGroups);
    }

    [Fact]
    public void SpatialNavigation_UsesDeliberateWalkingPace()
    {
        Assert.Equal(1.5, SpatialNavigationMicroBundle.DefaultMovementStep);
    }

    [Fact]
    public void MicroBundle_HubArbitrationConvergesAfterItsContribution()
    {
        using var bundle = new MicroBundle(1999, "Arbitration Test", new WebMicroBundleProvider());
        var hub = new SingularityHub();
        var contract = (TheSingularityWorkshop.SingularityHub.IMicroBundle)bundle;

        Assert.True(contract.Arbitrate(hub, 0));
        Assert.False(contract.Arbitrate(hub, 1));
    }

    public void Dispose() => TheSingularityWorkshop.FSM_API.FSM_API.Internal.ResetAPI(true);
}
