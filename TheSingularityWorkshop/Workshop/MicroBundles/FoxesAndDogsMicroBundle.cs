using TheSingularityWorkshop.FSM_COS;
using TheSingularityWorkshop.MicroBundleDomain;

namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>
/// Composition capability for the original Foxes & Dogs instructional demonstration.
/// The demonstration itself remains implemented by the FSM_API agent contexts; this
/// MicroBundle makes that capability discoverable through FSM_COS.
/// </summary>
public sealed class FoxesAndDogsMicroBundle : IMicroBundle
{
    public const int BundleId = 2101;

    public MicroBundleDescriptor Descriptor { get; } = new((ulong)BundleId, "1.0.0");
    public ulong Id => (ulong)BundleId;
    public IReadOnlyList<BundleRequest> Dependencies { get; } = Array.Empty<BundleRequest>();

    void IMicroBundle.Load(MicroBundleLoadContext context)
        => ArgumentNullException.ThrowIfNull(context);

    bool IMicroBundle.Arbitrate(ArbitrationContext context, int roundIndex)
    {
        ArgumentNullException.ThrowIfNull(context);
        return false;
    }
}
