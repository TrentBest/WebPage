using System;

namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>
/// Common lifecycle/facade implementation for a focused software-pattern MicroBundle.
/// Each concrete pattern remains its own MicroBundle and therefore remains independently
/// discoverable, composable, versionable, and presentable.
/// </summary>
public abstract class SoftwarePatternMicroBundle : IDisposable
{
    private readonly MicroBundle _lifecycle;
    private bool _disposed;

    protected SoftwarePatternMicroBundle(int id, string name, MicroBundlePresentation presentation)
    {
        _lifecycle = new MicroBundle(id, name, new WebMicroBundleProvider());
        Presentation = presentation;
    }

    public ulong Id => (ulong)_lifecycle.Id;
    public string Name => _lifecycle.Name;
    public MicroBundleManifestation? Manifestation => _lifecycle.Manifestation;
    public string Phase => ((MicroBundleContext)_lifecycle.Context).Phase;
    public MicroBundlePresentation Presentation { get; }

    public void Update()
    {
        if (_disposed) return;
        _lifecycle.Update();
    }

    public void Invalidate() => _lifecycle.Invalidate();

    public void Dispose()
    {
        if (_disposed) return;
        _lifecycle.Dispose();
        _disposed = true;
    }
}
