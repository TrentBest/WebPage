using TheSingularityWorkshop.FSM_COS;
using TheSingularityWorkshop.MicroBundleDomain;

namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>
/// Composition capability for the Living GUI Experience.
/// FSM_COS must load the Workshop Moniker before this Experience capability can install.
/// The actual living organism is then scheduled by the host's FSM_API process groups.
/// </summary>
public sealed class LivingGuiExperienceMicroBundle : TheSingularityWorkshop.FSM_COS.IMicroBundle
{
    public const int BundleId = 2102;

    public MicroBundleDescriptor Descriptor { get; } = new((ulong)BundleId, "1.0.0");
    public ulong Id => (ulong)BundleId;

    /// <summary>
    /// The Experience cannot be installed without the canonical Workshop Moniker.
    /// This is the architectural ordering guarantee requested for the first runtime.
    /// </summary>
    public IReadOnlyList<BundleRequest> Dependencies { get; } =
        [BundleRequest.Unconfigured((ulong)MonikerMicroBundle.BundleId)];

    void TheSingularityWorkshop.FSM_COS.IMicroBundle.Load(MicroBundleLoadContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        // Composition is intentionally semantic here. The browser manifestation
        // remains owned by LivingGui.razor; FSM_COS owns the installation order.
    }

    bool TheSingularityWorkshop.FSM_COS.IMicroBundle.Arbitrate(
        ArbitrationContext context,
        int roundIndex)
    {
        ArgumentNullException.ThrowIfNull(context);
        return false;
    }
}
