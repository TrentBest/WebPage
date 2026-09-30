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

    private static readonly MicroBundleRegistry SharedRegistry = new();
    private readonly MicroBundle _lifecycle;
    private bool _disposed;

    public ExploreExperienceMicroBundle()
    {
        _lifecycle = new MicroBundle(BundleId, "WORKSHOP EXPLORE EXPERIENCE", new WebMicroBundleProvider());
        Navigation = new SpatialNavigationMicroBundle();
        User = new UserMicroBundle();
        Conference = new ConferenceMicroBundle();
        Forge = new FsmForgeMicroBundle();
        Presence = new WorkshopPresenceMicroBundle();
        SoftwarePatterns = new SoftwarePatternsMicroBundle();
        SoftwarePatternsAnnotation = new SoftwarePatternsAnnotationMicroBundle(SoftwarePatterns);
        Moniker = GetOrLoadMoniker();
    }

    public ulong Id => (ulong)_lifecycle.Id;
    public MicroBundleManifestation? Manifestation => _lifecycle.Manifestation;
    public string Phase => ((MicroBundleContext)_lifecycle.Context).Phase;

    /// <summary>
    /// The shared Workshop moniker capability. Multiple Experiences resolve this
    /// to the same loaded bundle rather than constructing another copy.
    /// </summary>
    public MonikerMicroBundle Moniker { get; }

    public SpatialNavigationMicroBundle Navigation { get; }
    public UserMicroBundle User { get; }
    public ConferenceMicroBundle Conference { get; }
    public FsmForgeMicroBundle Forge { get; }
    public WorkshopPresenceMicroBundle Presence { get; }
    public SoftwarePatternsMicroBundle SoftwarePatterns { get; }
    public SoftwarePatternsAnnotationMicroBundle SoftwarePatternsAnnotation { get; }

    /// <summary>Advances the composed capability lifecycles without owning the shared moniker lifecycle.</summary>
    public void Update()
    {
        if (_disposed) return;
        _lifecycle.Update();
        Navigation.Update();
        User.Update();
        Conference.Update();
        Forge.Update();
        Presence.Update();
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
        Presence.Dispose();
        Conference.Dispose();
        User.Dispose();
        Navigation.Dispose();
        _lifecycle.Dispose();
        _disposed = true;
    }

    private static MonikerMicroBundle GetOrLoadMoniker()
    {
        var address = MicroBundleAddress.Create(
            new OntologySignature(0, 0, 0, 0, 0, 0, 0, 0, MonikerMicroBundle.BundleId),
            0);

        if (SharedRegistry.TryGet(address, out var loaded) && loaded is MonikerMicroBundle existing)
            return existing;

        var created = new MonikerMicroBundle();
        if (SharedRegistry.TryPublish(address, created))
            return created;

        // Another Experience may have populated the slot between lookup and publication.
        created.Dispose();
        return SharedRegistry.TryGet(address, out var winner) && winner is MonikerMicroBundle moniker
            ? moniker
            : throw new InvalidOperationException("The shared Workshop moniker could not be loaded.");
    }
}
