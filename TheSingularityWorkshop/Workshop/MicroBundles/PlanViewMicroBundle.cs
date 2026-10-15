using System;

namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>
/// Runnable WebPage MicroBundle shell for the renderer-neutral VBIMB plan capability.
/// The host chooses when to update/dispose it and how to present its manifestation.
/// </summary>
public sealed class PlanViewMicroBundle : IDisposable
{
    private readonly MicroBundle _lifecycle;

    public PlanViewMicroBundle()
        => _lifecycle = new MicroBundle(
            PlanViewMicroBundleProvider.BundleId,
            "VBIMB // PLAN VIEW",
            new PlanViewMicroBundleProvider());

    public MicroBundleManifestation? Manifestation => _lifecycle.Manifestation;

    public void Update() => _lifecycle.Update();

    public void Dispose() => _lifecycle.Dispose();
}
