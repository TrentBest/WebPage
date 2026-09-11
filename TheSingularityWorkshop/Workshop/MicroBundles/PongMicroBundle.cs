using TheSingularityWorkshop.SingularityHub;

namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>
/// The first concrete Workshop experience implemented as a MicroBundle.
/// Pong is intentionally unaware of idle mode; idle is only one host condition
/// in which this experience may be manifested.
/// </summary>
public sealed class PongMicroBundle
{
    public const int BundleId = 2001;

    private readonly MicroBundle _lifecycle;

    public PongMicroBundle()
    {
        _lifecycle = new MicroBundle(BundleId, "PONG", new WebMicroBundleProvider());
    }

    /// <summary>Stable MicroBundle identity projected by the underlying lifecycle.</summary>
    public ulong Id => (ulong)_lifecycle.Id;

    /// <summary>Current lifecycle manifestation produced by the MicroBundle provider.</summary>
    public MicroBundleManifestation? Manifestation => _lifecycle.Manifestation;

    /// <summary>Current FSM lifecycle phase.</summary>
    public string Phase => ((MicroBundleContext)_lifecycle.Context).Phase;

    /// <summary>
    /// Advances the bundle through its FSM_API-owned lifecycle and refreshes its manifestation.
    /// The Pong simulation itself remains a presentation concern of its host.
    /// </summary>
    public void Update() => _lifecycle.Update();

    /// <summary>Marks the bundle invalid so its FSM lifecycle can collapse it.</summary>
    public void Invalidate() => _lifecycle.Invalidate();
}

/// <summary>
/// Runtime catalog of installable Workshop MicroBundles.
/// This is deliberately separate from the idle catalog: any experience may be
/// loaded here regardless of why the host requested it.
/// </summary>
public static class MicroBundleCatalog
{
    private static readonly IReadOnlyList<ulong> AvailableIds = [PongMicroBundle.BundleId];

    /// <summary>Gets the MicroBundles available to the Workshop runtime.</summary>
    public static IReadOnlyList<ulong> Available => AvailableIds;

    /// <summary>Creates a fresh runtime instance for a registered MicroBundle.</summary>
    public static PongMicroBundle CreatePong()
    {
        return new PongMicroBundle();
    }

    /// <summary>Returns whether the supplied identity is currently available.</summary>
    public static bool IsAvailable(ulong id) => AvailableIds.Contains(id);
}
