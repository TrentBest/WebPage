using TheSingularityWorkshop.MicroBundleDomain;

namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>
/// Host-neutral identity for a behavior that can be manifested alongside the
/// stationary Workshop Moniker. Presentation hosts map this semantic choice to
/// their own rendering mechanism.
/// </summary>
public interface IMonikerPresentationBehavior : IMicroBundle
{
    string PresentationClass { get; }
}
