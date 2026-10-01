using System;
using System.Collections.Generic;
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
    private readonly FsmForgePreview _preview;
    private bool _disposed;

    public FsmForgeMicroBundle()
    {
        _lifecycle = new MicroBundle(BundleId, "FSM FORGE", new WebMicroBundleProvider());
        _preview = new FsmForgePreview();
        Workbench = new FsmForgeWorkbench();
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
    public FsmForgeWorkbench Workbench { get; }

    public string State => _preview.State;
    public int TickCount => _preview.TickCount;
    public IReadOnlyList<string> Events => _preview.Events;
    public bool IsRunning => _preview.IsRunning;

    public void Start()
    {
        if (!Workbench.Definition.CanPreview(out _))
            return;

        _preview.Start(Workbench.Definition);
    }

    public void Tick() => _preview.Tick();
    public void Reset()
    {
        _preview.Stop();
        Workbench.RebuildDefinition();
    }

    public void Update() => _lifecycle.Update();
    public void Invalidate() => _lifecycle.Invalidate();

    public void Dispose()
    {
        if (_disposed) return;
        _preview.Stop();
        _lifecycle.Dispose();
        _disposed = true;
    }
}
