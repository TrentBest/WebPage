using System;
using TheSingularityWorkshop.Infrastructure.FsmForge;
using TheSingularityWorkshop.SingularityHub;

namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>
/// Exposes the FSM Forge capability as a MicroBundle so an Experience consumes
/// the Forge rather than constructing its implementation directly.
/// The bundle is also the Forge facade: hosts consume capability state and
/// commands here instead of reaching through to the infrastructure preview.
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
        Presentation = new MicroBundlePresentation(
            "The Forge",
            "FSM WORKSHOP",
            "Shape state, lifecycle, reusable substrates, and executable machinery.",
            ["STATE LIFECYCLE", "FSM STRUCTURE", "RUNTIME", "LIVE PREVIEW"],
            "Magenta");
    }

    public ulong Id => (ulong)_lifecycle.Id;
    public MicroBundleManifestation? Manifestation => _lifecycle.Manifestation;
    public string Phase => ((MicroBundleContext)_lifecycle.Context).Phase;
    public MicroBundlePresentation Presentation { get; }

    public string State => Preview.State;
    public int TickCount => Preview.TickCount;
    public IReadOnlyList<string> Events => Preview.Events;
    public bool IsRunning => Preview.IsRunning;

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
