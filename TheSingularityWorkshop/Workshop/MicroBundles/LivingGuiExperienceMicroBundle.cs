using TheSingularityWorkshop.MicroBundleDomain;
using TheSingularityWorkshop.Workshop.DeepDive;

namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>
/// Composition capability for the Living GUI Experience.
/// FSM_COS must load the Workshop Moniker before this Experience capability can install.
/// The actual living organism is then scheduled by the host's FSM_API process groups.
/// </summary>
public sealed class LivingGuiExperienceMicroBundle :
    TheSingularityWorkshop.MicroBundleDomain.IMicroBundle,
    IWebPageProviderSource
{
    public const int BundleId = 2102;

    public MicroBundleDescriptor Descriptor { get; } = new((ulong)BundleId, "1.0.0");
    public ulong Id => (ulong)BundleId;

    /// <summary>
    /// The Experience cannot be installed without the canonical Workshop Moniker.
    /// This is the architectural ordering guarantee requested for the first runtime.
    /// </summary>
    public IReadOnlyList<MicroBundleDependencyRequest> Dependencies { get; } =
        [MicroBundleDependencyRequest.Unconfigured((ulong)MonikerMicroBundle.BundleId)];

    /// <summary>Acquires an optional provider from the WebPage-only provider collection.</summary>
    public T? TryGetProvider<T>() where T : class
        => typeof(T) == typeof(LivingGuiDeepDiveProvider) || typeof(T) == typeof(IDeepDiveProvider)
            ? (T)(object)new LivingGuiDeepDiveProvider()
            : null;

    public void Load(IMicroBundleLoadContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
    }

    public bool Arbitrate(
        IMicroBundleArbitrationContext context,
        int roundIndex)
    {
        ArgumentNullException.ThrowIfNull(context);
        return false;
    }
}
