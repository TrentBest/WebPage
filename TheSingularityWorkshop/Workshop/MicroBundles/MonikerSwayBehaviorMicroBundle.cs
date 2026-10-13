using System;
using System.Collections.Generic;
using TheSingularityWorkshop.MicroBundleDomain;
using TheSingularityWorkshop.Workshop.Gui;

namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>
/// Declares the canonical kelp-sway presentation behavior for the Workshop Moniker.
/// The stationary Moniker remains owned by MonikerMicroBundle; this bundle describes
/// the optional behavior that a host-specific presentation adapter can manifest.
/// </summary>
public sealed class MonikerSwayBehaviorMicroBundle : TheSingularityWorkshop.MicroBundleDomain.IMicroBundle, IMonikerPresentationBehavior
{
    public const int BundleId = 2111;
    public const string PresentationClass = "moniker-kelp-sway";
    string IMonikerPresentationBehavior.PresentationClass => PresentationClass;

    public MicroBundleDescriptor Descriptor { get; } = new((ulong)BundleId, "1.0.0");
    public ulong Id => (ulong)BundleId;
    public IReadOnlyList<MicroBundleDependencyRequest> Dependencies { get; } =
        Array.Empty<MicroBundleDependencyRequest>();

    /// <summary>A semantic description of this behavior for inspection and adaptation.</summary>
    public GuiNode? Composition { get; private set; }

    public void Load(IMicroBundleLoadContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        Composition = GuiBuilder.Create("Panel", "moniker-sway-behavior")
            .Property("capability", "moniker.behavior.sway")
            .Property("presentationClass", PresentationClass)
            .Property("motion", "slow-bidirectional-kelp")
            .Build();
    }

    public bool Arbitrate(IMicroBundleArbitrationContext context, int roundIndex)
    {
        ArgumentNullException.ThrowIfNull(context);
        return false;
    }
}
