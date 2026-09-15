using System;
using TheSingularityWorkshop.SingularityHub;

namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>
/// Composition root for the spatial Workshop Experience.
/// An Experience is not a page full of bespoke behavior: it is a composition of
/// MicroBundles that can expose capabilities, state, and presentation contracts.
/// Humans remain in the loop by choosing where to go, what to enter, and which
/// capabilities to activate.
/// </summary>
public sealed class ExploreExperienceMicroBundle : IDisposable
{
    public const int BundleId = 2100;

    private readonly MicroBundle _lifecycle;
    private bool _disposed;

    public ExploreExperienceMicroBundle()
    {
        _lifecycle = new MicroBundle(BundleId, "WORKSHOP EXPLORE EXPERIENCE", new WebMicroBundleProvider());
        Navigation = new SpatialNavigationMicroBundle();
        User = new UserMicroBundle();
        Conference = new ConferenceMicroBundle();
        Forge = new FsmForgeMicroBundle();
        SoftwarePatterns = new SoftwarePatternsMicroBundle();
        SoftwarePatternsAnnotation = new SoftwarePatternsAnnotationMicroBundle(SoftwarePatterns);
    }

    public ulong Id => (ulong)_lifecycle.Id;
    public MicroBundleManifestation? Manifestation => _lifecycle.Manifestation;
    public string Phase => ((MicroBundleContext)_lifecycle.Context).Phase;

    public SpatialNavigationMicroBundle Navigation { get; }
    public UserMicroBundle User { get; }
    public ConferenceMicroBundle Conference { get; }
    public FsmForgeMicroBundle Forge { get; }
    public SoftwarePatternsMicroBundle SoftwarePatterns { get; }
    public SoftwarePatternsAnnotationMicroBundle SoftwarePatternsAnnotation { get; }

    /// <summary>Advances the composed capability lifecycles without owning their presentation.</summary>
    public void Update()
    {
        if (_disposed) return;
        _lifecycle.Update();
        Navigation.Update();
        User.Update();
        Conference.Update();
        Forge.Update();
        SoftwarePatterns.Update();
        SoftwarePatternsAnnotation.Update();
    }

    public void Invalidate() => _lifecycle.Invalidate();

    public void Dispose()
    {
        if (_disposed) return;
        SoftwarePatternsAnnotation.Dispose();
        SoftwarePatterns.Dispose();
        Forge.Dispose();
        Conference.Dispose();
        User.Dispose();
        Navigation.Dispose();
        _lifecycle.Dispose();
        _disposed = true;
    }
}
