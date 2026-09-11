using TheSingularityWorkshop.FSM_API;
using TheSingularityWorkshop.Workshop.MicroBundles;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>
/// Layer 2 lifecycle coverage for the concrete MicroBundle Element.
/// FSM_API remains the lifecycle authority; tests advance it explicitly.
/// </summary>
public sealed class MicroBundleTests : IDisposable
{
    public MicroBundleTests() => Reset();

    [ArchitectureTest(2, 2, 1)]
    [Fact(DisplayName = "2.02.001 — NewBundle_StartsCreated")]
    public void NewBundle_StartsCreated()
    {
        var bundle = CreateBundle(1001);
        var context = Context(bundle);
        Assert.Equal("Created", context.Phase);
        Assert.Equal(0, context.ElapsedMilliseconds);
        Assert.Null(bundle.Manifestation);
    }

    [ArchitectureTest(2, 2, 2)]
    [Fact(DisplayName = "2.02.002 — FirstUpdate_ProducesManifesting")]
    public void FirstUpdate_ProducesManifesting()
    {
        var bundle = CreateBundle(1002);
        bundle.Update();
        Assert.Equal("Manifesting", Context(bundle).Phase);
        Assert.Equal("Trace", bundle.Manifestation!.Effect);
    }

    [ArchitectureTest(2, 2, 3)]
    [Fact(DisplayName = "2.02.003 — SecondUpdate_ProducesActive")]
    public void SecondUpdate_ProducesActive()
    {
        var bundle = CreateBundle(1003);
        bundle.Update();
        bundle.Update();
        Assert.Equal("Active", Context(bundle).Phase);
        Assert.Equal("Breathe", bundle.Manifestation!.Effect);
    }

    [ArchitectureTest(2, 2, 4)]
    [Fact(DisplayName = "2.02.004 — Active_RemainsActive_WhileValid")]
    public void Active_RemainsActive_WhileValid()
    {
        var bundle = CreateBundle(1004);
        bundle.Update();
        bundle.Update();
        bundle.Update();
        Assert.Equal("Active", Context(bundle).Phase);
        Assert.Equal(3, Context(bundle).ElapsedMilliseconds);
    }

    [ArchitectureTest(2, 2, 5)]
    [Fact(DisplayName = "2.02.005 — Invalidate_SetsElementInvalid")]
    public void Invalidate_SetsElementInvalid()
    {
        var bundle = CreateBundle(1005);
        bundle.Invalidate();
        Assert.False(Context(bundle).IsValid);
    }

    [ArchitectureTest(2, 2, 6)]
    [Fact(DisplayName = "2.02.006 — Invalidate_Active_EntersCollapsing")]
    public void Invalidate_Active_EntersCollapsing()
    {
        var bundle = CreateBundle(1006);
        bundle.Update();
        bundle.Update();
        bundle.Invalidate();
        bundle.Update();
        Assert.Equal("Collapsing", Context(bundle).Phase);
        Assert.Equal("Collapse", bundle.Manifestation!.Effect);
    }

    [ArchitectureTest(2, 2, 7)]
    [Fact(DisplayName = "2.02.007 — Collapsing_EntersDestroyed_OnNextTick")]
    public void Collapsing_EntersDestroyed_OnNextTick()
    {
        var bundle = CreateBundle(1007);
        bundle.Update();
        bundle.Update();
        bundle.Invalidate();
        bundle.Update();
        bundle.Update();
        Assert.Equal("Destroyed", Context(bundle).Phase);
        Assert.Equal("Remove", bundle.Manifestation!.Effect);
    }

    [ArchitectureTest(2, 2, 8)]
    [Fact(DisplayName = "2.02.008 — Destroyed_RemainsDestroyed")]
    public void Destroyed_RemainsDestroyed()
    {
        var bundle = CreateBundle(1008);
        bundle.Update();
        bundle.Update();
        bundle.Invalidate();
        bundle.Update();
        bundle.Update();
        bundle.Update();
        Assert.Equal("Destroyed", Context(bundle).Phase);
    }

    [ArchitectureTest(2, 2, 9)]
    [Fact(DisplayName = "2.02.009 — ParentAndGeneration_Are_Preserved")]
    public void ParentAndGeneration_Are_Preserved()
    {
        var bundle = new MicroBundle(1009, "Lineage", new WebMicroBundleProvider(), 77, 4);
        var context = Context(bundle);
        Assert.Equal(77, context.ParentId);
        Assert.Equal(4, context.Generation);
    }

    [ArchitectureTest(2, 2, 10)]
    [Fact(DisplayName = "2.02.010 — Manifestation_Tracks_Elapsed_Time")]
    public void Manifestation_Tracks_Elapsed_Time()
    {
        var bundle = CreateBundle(1010);
        bundle.Update();
        Assert.Equal(1, bundle.Manifestation!.Parameters["Elapsed"]);
        bundle.Update();
        Assert.Equal(2, bundle.Manifestation!.Parameters["Elapsed"]);
    }

    [ArchitectureTest(2, 2, 11)]
    [Fact(DisplayName = "2.02.011 — Element_Identity_Is_Stable")]
    public void Element_Identity_Is_Stable()
    {
        var bundle = CreateBundle(1011);
        Assert.Equal(1011, bundle.Id);
        Assert.Equal(1011, Context(bundle).Id);
    }

    [ArchitectureTest(2, 2, 12)]
    [Fact(DisplayName = "2.02.012 — Element_Name_Reaches_Context")]
    public void Element_Name_Reaches_Context()
    {
        var bundle = CreateBundle(1012);
        Assert.Equal("TestElement_1012", Context(bundle).Name);
    }

    [ArchitectureTest(2, 2, 13)]
    [Fact(DisplayName = "2.02.013 — Explicit_Update_Is_The_Only_Lifecycle_Clock")]
    public void Explicit_Update_Is_The_Only_Lifecycle_Clock()
    {
        var bundle = CreateBundle(1013);
        var context = Context(bundle);
        Assert.Equal("Created", context.Phase);
        Assert.Equal(0, context.ElapsedMilliseconds);
        Assert.Null(bundle.Manifestation);
    }

    [ArchitectureTest(2, 2, 14)]
    [Fact(DisplayName = "2.02.014 — Separate_Bundles_Do_Not_Share_Context")]
    public void Separate_Bundles_Do_Not_Share_Context()
    {
        var first = CreateBundle(1014);
        var second = CreateBundle(1015);
        Assert.NotSame(first.Context, second.Context);
        Assert.NotSame(first, second);
    }

    [ArchitectureTest(2, 2, 15)]
    [Fact(DisplayName = "2.02.015 — Reset_Allows_Fresh_Element")]
    public void Reset_Allows_Fresh_Element()
    {
        _ = CreateBundle(1016);
        Reset();
        var fresh = CreateBundle(1017);
        Assert.Equal("Created", Context(fresh).Phase);
        Assert.Equal(0, Context(fresh).ElapsedMilliseconds);
    }

    public void Dispose() => Reset();

    private static MicroBundle CreateBundle(int id)
        => new(id, $"TestElement_{id}", new WebMicroBundleProvider());

    private static MicroBundleContext Context(MicroBundle bundle)
        => Assert.IsType<MicroBundleContext>(bundle.Context);

    private static void Reset() => FSM_API.Internal.ResetAPI(true);
}
