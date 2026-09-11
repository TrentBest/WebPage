using TheSingularityWorkshop.Workshop.MicroBundles;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>
/// TDD contract for the Atom → Element boundary.
///
/// An Atom is an FSM-backed runtime unit. An Element is a MicroBundle whose
/// lifecycle is owned by that FSM. The provider only manifests the current
/// lifecycle state; it does not drive the state machine.
/// </summary>
public sealed class MicroBundleTests
{
    [Fact]
    public void NewBundle_StartsCreated_AndDoesNotManifestUntilUpdated()
    {
        var bundle = CreateBundle(1001);
        var context = Assert.IsType<MicroBundleContext>(bundle.Context);

        Assert.Equal("Created", context.Phase);
        Assert.Equal(0, context.ElapsedMilliseconds);
        Assert.Null(bundle.Manifestation);
    }

    [Fact]
    public void FirstUpdate_DrivesCreatedToManifesting_ThroughFSM()
    {
        var bundle = CreateBundle(1002);

        bundle.Update();

        var context = Assert.IsType<MicroBundleContext>(bundle.Context);
        Assert.Equal("Manifesting", context.Phase);
        Assert.Equal(1, context.ElapsedMilliseconds);
        Assert.Equal("Trace", bundle.Manifestation!.Effect);
    }

    [Fact]
    public void SecondUpdate_DrivesManifestingToActive_AndProviderFollowsFSM()
    {
        var bundle = CreateBundle(1003);

        bundle.Update();
        bundle.Update();

        var context = Assert.IsType<MicroBundleContext>(bundle.Context);
        Assert.Equal("Active", context.Phase);
        Assert.Equal(2, context.ElapsedMilliseconds);
        Assert.Equal("Breathe", bundle.Manifestation!.Effect);
    }

    [Fact]
    public void Invalidation_DrivesActiveToCollapsing_AndThenDestroyed()
    {
        var bundle = CreateBundle(1004);

        bundle.Update();
        bundle.Update();
        Assert.Equal("Active", ((MicroBundleContext)bundle.Context).Phase);

        bundle.Invalidate();
        bundle.Update();

        var context = Assert.IsType<MicroBundleContext>(bundle.Context);
        Assert.False(context.IsValid);
        Assert.Equal("Collapsing", context.Phase);
        Assert.Equal("Collapse", bundle.Manifestation!.Effect);

        bundle.Update();

        Assert.Equal("Destroyed", context.Phase);
        Assert.Equal("Remove", bundle.Manifestation!.Effect);
    }

    [Fact]
    public void Provider_ManifestsElementState_ButDoesNotOwnLifecycle()
    {
        var bundle = CreateBundle(1005);
        var provider = new WebMicroBundleProvider();

        var before = provider.Manifest(bundle.Context);
        Assert.Equal("Dormant", before.Effect);

        bundle.Update();

        var after = provider.Manifest(bundle.Context);
        Assert.Equal("Manifesting", ((MicroBundleContext)bundle.Context).Phase);
        Assert.Equal("Trace", after.Effect);
    }

    private static MicroBundle CreateBundle(int id)
        => new(id, $"TestElement_{id}", new WebMicroBundleProvider());
}
