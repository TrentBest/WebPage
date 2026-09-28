using TheSingularityWorkshop.FSM_API;
using TheSingularityWorkshop.Workshop.MicroBundles;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>Layer 3: Provider. Providers translate state into manifestation; they do not own lifecycle.</summary>
public sealed class ProviderTests
{
    private readonly WebMicroBundleProvider _provider = new();

    [ArchitectureTest(3, 1, 1)]
    [Fact(DisplayName = "3.01.001 — Provider_Implements_Contract")]
    public void Provider_Implements_Contract()
        => Assert.IsAssignableFrom<IMicroBundleProvider>(_provider);

    [ArchitectureTest(3, 1, 2)]
    [Fact(DisplayName = "3.01.002 — Created_Produces_Dormant")]
    public void Created_Produces_Dormant()
    {
        var context = new MicroBundleContext(3002, "Created");
        var manifestation = _provider.Manifest(context);
        Assert.Equal("Dormant", manifestation.Effect);
    }

    [ArchitectureTest(3, 1, 3)]
    [Fact(DisplayName = "3.01.003 — Manifesting_Produces_Trace")]
    public void Manifesting_Produces_Trace()
    {
        var context = new MicroBundleContext(3003, "Manifesting") { Phase = "Manifesting" };
        Assert.Equal("Trace", _provider.Manifest(context).Effect);
    }

    [ArchitectureTest(3, 1, 4)]
    [Fact(DisplayName = "3.01.004 — Active_Produces_Breathe")]
    public void Active_Produces_Breathe()
    {
        var context = new MicroBundleContext(3004, "Active") { Phase = "Active" };
        Assert.Equal("Breathe", _provider.Manifest(context).Effect);
    }

    [ArchitectureTest(3, 1, 5)]
    [Fact(DisplayName = "3.01.005 — Collapsing_Produces_Collapse")]
    public void Collapsing_Produces_Collapse()
    {
        var context = new MicroBundleContext(3005, "Collapsing") { Phase = "Collapsing" };
        Assert.Equal("Collapse", _provider.Manifest(context).Effect);
    }

    [ArchitectureTest(3, 1, 6)]
    [Fact(DisplayName = "3.01.006 — Destroyed_Produces_Remove")]
    public void Destroyed_Produces_Remove()
    {
        var context = new MicroBundleContext(3006, "Destroyed") { Phase = "Destroyed" };
        Assert.Equal("Remove", _provider.Manifest(context).Effect);
    }

    [ArchitectureTest(3, 1, 7)]
    [Fact(DisplayName = "3.01.007 — Provider_Preserves_Context_State")]
    public void Provider_Preserves_Context_State()
    {
        var context = new MicroBundleContext(3007, "Provider")
        {
            Phase = "Active",
            ElapsedMilliseconds = 17,
            IsValid = false,
            IsInvalidated = false
        };

        _ = _provider.Manifest(context);

        Assert.Equal("Active", context.Phase);
        Assert.Equal(17, context.ElapsedMilliseconds);
        Assert.False(context.IsValid);
        Assert.False(context.IsInvalidated);
    }

    [ArchitectureTest(3, 1, 8)]
    [Fact(DisplayName = "3.01.008 — Provider_Preserves_Element_Identity")]
    public void Provider_Preserves_Element_Identity()
    {
        var context = new MicroBundleContext(3008, "Identity") { Phase = "Active", ParentId = 41, Generation = 9 };
        var manifestation = _provider.Manifest(context);
        Assert.Equal(3008, manifestation.Id);
        Assert.Equal("Identity", manifestation.Target);
        Assert.Equal(41, manifestation.Parameters["ParentId"]);
        Assert.Equal(9, manifestation.Parameters["Generation"]);
    }

    [ArchitectureTest(3, 1, 9)]
    [Fact(DisplayName = "3.01.009 — Provider_Reports_Elapsed_Time")]
    public void Provider_Reports_Elapsed_Time()
    {
        var context = new MicroBundleContext(3009, "Clock") { Phase = "Active", ElapsedMilliseconds = 123 };
        Assert.Equal(123, _provider.Manifest(context).Parameters["Elapsed"]);
    }

    [ArchitectureTest(3, 1, 10)]
    [Fact(DisplayName = "3.01.010 — Provider_Uses_Semantic_Effects_Not_Host_UI")]
    public void Provider_Uses_Semantic_Effects_Not_Host_UI()
    {
        var method = typeof(WebMicroBundleProvider).GetMethod(nameof(WebMicroBundleProvider.Manifest));
        Assert.NotNull(method);
        Assert.Equal(typeof(MicroBundleManifestation), method!.ReturnType);
    }
}
