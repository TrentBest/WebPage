using System;
using System.Collections.Generic;

namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>
/// A focused grouping MicroBundle. It composes pattern MicroBundles without
/// collapsing their identities; the children remain independently addressable.
/// </summary>
public abstract class SoftwarePatternGroupMicroBundle : IDisposable
{
    private readonly MicroBundle _lifecycle;
    private bool _disposed;

    protected SoftwarePatternGroupMicroBundle(
        int id,
        string name,
        MicroBundlePresentation presentation,
        IReadOnlyList<SoftwarePatternMicroBundle> patterns)
    {
        _lifecycle = new MicroBundle(id, name, new WebMicroBundleProvider());
        Name = name;
        Presentation = presentation;
        Patterns = patterns;
    }

    public ulong Id => (ulong)_lifecycle.Id;
    public string Name { get; }
    public MicroBundlePresentation Presentation { get; }
    public IReadOnlyList<SoftwarePatternMicroBundle> Patterns { get; }
    public MicroBundleManifestation? Manifestation => _lifecycle.Manifestation;
    public string Phase => ((MicroBundleContext)_lifecycle.Context).Phase;

    public void Update()
    {
        if (_disposed) return;
        _lifecycle.Update();
        foreach (var pattern in Patterns)
            pattern.Update();
    }

    public void Invalidate()
    {
        if (_disposed) return;
        _lifecycle.Invalidate();
        foreach (var pattern in Patterns)
            pattern.Invalidate();
    }

    public void Dispose()
    {
        if (_disposed) return;
        for (var index = Patterns.Count - 1; index >= 0; index--)
            Patterns[index].Dispose();
        _lifecycle.Dispose();
        _disposed = true;
    }
}
