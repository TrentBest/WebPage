using System;
using TheSingularityWorkshop.Infrastructure.FsmForge;
using TheSingularityWorkshop.SingularityHub;

namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>
/// Exposes the FSM Forge capability as a MicroBundle so an Experience consumes
/// the Forge rather than constructing its implementation directly.
/// </summary>
public sealed class FsmForgeMicroBundle : IDisposable
{
    public const int BundleId = 2103;

    private readonly MicroBundle _lifecycle;
    private bool _disposed;

    public FsmForgeMicroBundle()
    {
        _lifecycle = new MicroBundle(BundleId, "FSM FORGE", new WebMicroBundleProvider());
        Preview = new FsmForgePreview();
    }

    public ulong Id => (ulong)_lifecycle.Id;
    public MicroBundleManifestation? Manifestation => _lifecycle.Manifestation;
    public string Phase => ((MicroBundleContext)_lifecycle.Context).Phase;
    public FsmForgePreview Preview { get; }

    public void Start() => Preview.Start();
    public void Tick() => Preview.Tick();
    public void Reset() => Preview.Stop();
    public void Update() => _lifecycle.Update();
    public void Invalidate() => _lifecycle.Invalidate();

    public void Dispose()
    {
        if (_disposed) return;
        Preview.Stop();
        _lifecycle.Dispose();
        _disposed = true;
    }
}
