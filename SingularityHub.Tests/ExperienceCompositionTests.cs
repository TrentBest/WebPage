using TheSingularityWorkshop.SingularityHub;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>Contract tests for manifest-driven Experience composition.</summary>
public sealed class ExperienceCompositionTests
{
    [ArchitectureTest(1, 2, 1)]
    [Fact(DisplayName = "1.02.001 — Manifest_Preserves_Explicit_Phase_Order")]
    public void Manifest_Preserves_Explicit_Phase_Order()
    {
        var manifest = new ExperienceManifest(
            new[] { 30UL, 10UL, 20UL },
            new[] { 40UL },
            new[] { 50UL, 60UL },
            new[] { HubCapabilityIds.Moniker });

        Assert.Equal(new[] { 30UL, 10UL, 20UL }, manifest.Startup);
        Assert.Equal(new[] { 40UL }, manifest.Transitioning);
        Assert.Equal(new[] { 50UL, 60UL }, manifest.Running);
    }

    [ArchitectureTest(1, 2, 2)]
    [Fact(DisplayName = "1.02.002 — Manifest_Can_Require_Moniker_Capability")]
    public void Manifest_Can_Require_Moniker_Capability()
    {
        var manifest = new ExperienceManifest(
            Array.Empty<ulong>(),
            Array.Empty<ulong>(),
            Array.Empty<ulong>(),
            new[] { HubCapabilityIds.Moniker });

        Assert.True(manifest.RequiresCapability(HubCapabilityIds.Moniker));
    }

    [ArchitectureTest(1, 2, 3)]
    [Fact(DisplayName = "1.02.003 — Manifest_Searches_All_Execution_Phases")]
    public void Manifest_Searches_All_Execution_Phases()
    {
        var manifest = new ExperienceManifest(
            new[] { 10UL },
            new[] { 20UL },
            new[] { 30UL },
            Array.Empty<ulong>());

        Assert.True(manifest.ContainsExperience(10));
        Assert.True(manifest.ContainsExperience(20));
        Assert.True(manifest.ContainsExperience(30));
        Assert.False(manifest.ContainsExperience(99));
    }

    [ArchitectureTest(1, 2, 4)]
    [Fact(DisplayName = "1.02.004 — Presentation_And_Removal_Are_Independent_MicroBundles")]
    public void Presentation_And_Removal_Are_Independent_MicroBundles()
    {
        var presentation = new TestPresentation(new[] { 100UL, 101UL }, new[] { 200UL, 201UL });

        Assert.Equal(new[] { 100UL, 101UL }, presentation.PresentationMicroBundleIds);
        Assert.Equal(new[] { 200UL, 201UL }, presentation.RemovalMicroBundleIds);
    }

    private sealed record TestPresentation(
        IReadOnlyList<ulong> PresentationMicroBundleIds,
        IReadOnlyList<ulong> RemovalMicroBundleIds) : IExperiencePresentation;
}
