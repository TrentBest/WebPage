using System;
using System.Collections.Generic;
using TheSingularityWorkshop.MicroBundleDomain;

namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>
/// The default WebPage Moniker experience: a stationary canonical Moniker composed
/// with a separately selectable presentation behavior. The experience is the root;
/// neither identity nor motion is baked into this composition bundle.
/// </summary>
public sealed class MonikerExperienceMicroBundle : TheSingularityWorkshop.MicroBundleDomain.IMicroBundle
{
    public const int BundleId = 2101;

    public MicroBundleDescriptor Descriptor { get; } = new((ulong)BundleId, "1.0.0");
    public ulong Id => (ulong)BundleId;

    public IReadOnlyList<MicroBundleDependencyRequest> Dependencies { get; } =
    [
        MicroBundleDependencyRequest.Unconfigured((ulong)MonikerMicroBundle.BundleId),
        MicroBundleDependencyRequest.Unconfigured((ulong)MonikerSwayBehaviorMicroBundle.BundleId)
    ];

    public void Load(IMicroBundleLoadContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
    }

    public bool Arbitrate(IMicroBundleArbitrationContext context, int roundIndex)
    {
        ArgumentNullException.ThrowIfNull(context);
        return false;
    }
}
