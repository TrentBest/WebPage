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

    public ulong Id => (ulong)_lifecycle.Id;
    public MicroBundleManifestation? Manifestation => _lifecycle.Manifestation;
    public string Phase => ((MicroBundleContext)_lifecycle.Context).Phase;
    public void Update() => _lifecycle.Update();
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

    public static IReadOnlyList<ulong> Available => AvailableIds;

    public static PongMicroBundle CreatePong()
    {
        return new PongMicroBundle();
    }

    public static bool IsAvailable(ulong id) => AvailableIds.Contains(id);
}
