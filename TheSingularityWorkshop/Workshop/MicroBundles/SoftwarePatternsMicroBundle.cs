using System;
using System.Collections.Generic;
using System.Linq;

namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>
/// Top-level MicroBundle for the software-pattern capability family.
/// It composes the pattern groups while preserving every focused MicroBundle identity.
/// </summary>
public sealed class SoftwarePatternsMicroBundle : IDisposable
{
    public const int BundleId = 2213;

    private readonly MicroBundle _lifecycle;
    private bool _disposed;

    public SoftwarePatternsMicroBundle()
    {
        _lifecycle = new MicroBundle(
            BundleId,
            "SOFTWARE PATTERNS",
            new WebMicroBundleProvider());

        Creational = new CreationalPatternsMicroBundle();
        Structural = new StructuralPatternsMicroBundle();
        Behavioral = new BehavioralPatternsMicroBundle();
        Groups = [Creational, Structural, Behavioral];
    }

    public ulong Id => (ulong)_lifecycle.Id;
    public MicroBundleManifestation? Manifestation => _lifecycle.Manifestation;
    public string Phase => ((MicroBundleContext)_lifecycle.Context).Phase;

    public CreationalPatternsMicroBundle Creational { get; }
    public StructuralPatternsMicroBundle Structural { get; }
    public BehavioralPatternsMicroBundle Behavioral { get; }
    public IReadOnlyList<SoftwarePatternGroupMicroBundle> Groups { get; }

    /// <summary>All focused pattern MicroBundles reachable through the group hierarchy.</summary>
    public IReadOnlyList<SoftwarePatternMicroBundle> Patterns
        => Groups.SelectMany(group => group.Patterns).ToArray();

    public void Update()
    {
        if (_disposed) return;
        _lifecycle.Update();
        foreach (var group in Groups)
            group.Update();
    }

    public void Invalidate()
    {
        if (_disposed) return;
        _lifecycle.Invalidate();
        foreach (var group in Groups)
            group.Invalidate();
    }

    public void Dispose()
    {
        if (_disposed) return;
        for (var index = Groups.Count - 1; index >= 0; index--)
            Groups[index].Dispose();
        _lifecycle.Dispose();
        _disposed = true;
    }
}
